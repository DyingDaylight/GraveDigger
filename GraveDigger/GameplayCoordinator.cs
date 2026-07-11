using System;
using System.Collections.Generic;
using GraveDigger.Data;
using GraveDigger.Enemies;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Utils;
using GUI;
using Interfaces;

namespace GraveDigger;

public class GameplayCoordinator : IGameplayActions
{
    public IGameWindowService WindowService { get; }

    private readonly ReputationsSystem ReputationsSystem;
    private readonly RandomService RandomService;
    private readonly LootGenerator LootGenerator;
    private readonly Inventory Inventory;

    public event Action<List<ItemData>, Tombstone> OnLootSpawn;
    
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
            OnLootSpawn?.Invoke(itemData, tombstone);
            
            EnemyType enemyType = UndeadGenerator.Generate(tombstone.Data, RandomService);
            if (enemyType == EnemyType.Ghost)
            {
                ReputationsSystem.RemoveReputation(1);
                Console.WriteLine("Ghost appeared! Reputation reduced by 1!");
            } 
            else if (enemyType == EnemyType.Zombie)
            {
                Console.WriteLine("Zombie appeared! He will be eating you!");
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

    public void PickupItem(ItemData itemData)
    {
        Inventory.Add(itemData);
    }
    
    public void ShowInventory()
    {
        Console.WriteLine(Inventory.ToString());
    }
}