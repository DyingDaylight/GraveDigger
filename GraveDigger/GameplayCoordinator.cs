using System;
using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Props;
using GUI;
using Interfaces;

namespace GraveDigger;

public class GameplayCoordinator : IGameplayActions
{
    public IGameWindowService WindowService { get; }
    
    private ReputationsSystem ReputationsSystem { get; }
    
    private LootGenerator LootGenerator { get; }
    private Inventory Inventory { get; }

    public GameplayCoordinator(IGameWindowService windowService, ReputationsSystem reputationsSystem)
    {
        WindowService = windowService;
        ReputationsSystem = reputationsSystem;
        
        LootGenerator = new LootGenerator();
        Inventory = new Inventory();
    }

    public void OpenTombstone(Tombstone tombstoneData)
    {
        WindowService.OpenTombstoneWindow(tombstoneData);
        
    }

    public void DigGrave(Tombstone tombstone)
    {
        bool dug = tombstone.Dig();
        if (dug)
        {
            ItemData itemData = LootGenerator.Generate(tombstone.Data);
            Inventory.Add(itemData);
            ReputationsSystem.RemoveReputation(tombstone.Value);
            WindowService.CloseCurrentWindow();
        }
        Console.WriteLine("Digging Grave");
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