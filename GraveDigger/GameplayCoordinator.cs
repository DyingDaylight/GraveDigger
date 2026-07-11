using System;
using System.Collections.Generic;
using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Utils;
using GUI;
using Interfaces;

namespace GraveDigger;

public class GameplayCoordinator : IGameplayActions
{
    public IGameWindowService WindowService { get; }
    
    private ReputationsSystem ReputationsSystem { get; }
    
    private RandomService RandomService { get; }
    
    private LootGenerator LootGenerator { get; }
    private Inventory Inventory { get; }

    public GameplayCoordinator(IGameWindowService windowService, ReputationsSystem reputationsSystem,
        RandomService randomService)
    {
        WindowService = windowService;
        ReputationsSystem = reputationsSystem;
        RandomService = randomService;
        
        LootGenerator = new LootGenerator();
        Inventory = new Inventory();
    }

    public void OpenTombstone(Tombstone tombstoneData)
    {
        WindowService.OpenTombstoneWindow(tombstoneData);
        
    }

    public void DigGrave(Tombstone tombstone)
    {
        Console.WriteLine("Digging Grave");
        bool dug = tombstone.Dig();
        if (dug)
        {
            List<ItemData> itemData = LootGenerator.Generate(tombstone.Data, RandomService);
            Console.WriteLine("Looted " + itemData.Count + " items");
            foreach (ItemData item in itemData)
            {
                Console.WriteLine(item.ToString());
                Inventory.Add(item);   
            }
            ReputationsSystem.RemoveReputation(tombstone.Value);
            WindowService.CloseCurrentWindow();
        }
    }

    public void RepairGrave(Tombstone tombstone)
    {
        // TODO: check if we have resources
        bool repaired = tombstone.Repair();
        if (repaired)
        {
            ReputationsSystem.AddReputation(tombstone.Value);
            WindowService.UpdateTombstoneWindow();
        }
    }

    public void ShowInventory()
    {
        Console.WriteLine(Inventory.ToString());
    }
}