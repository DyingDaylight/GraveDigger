using System;
using System.Collections.Generic;
using GraveDigger.Enemies;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Sys;
using GraveDigger.Utils;
using Interfaces;

namespace GraveDigger;

public class GameplayCoordinator : IGameplayActions
{
    private readonly ReputationsSystem reputationsSystem;
    private readonly IGameWindowService windowService;
    private readonly RandomService randomService;
    private readonly LootGenerator lootGenerator;
    private readonly Inventory inventory;

    public event Action<List<ItemData>, Tombstone> OnLootSpawn;
    public event Action<Tombstone> OnGraveDug;
    public event Action<Tombstone> OnGraveRepaired;
    
    public GameplayCoordinator(IGameWindowService windowService, ReputationsSystem reputationsSystem,
        RandomService randomService)
    {
        this.windowService = windowService;
        this.reputationsSystem = reputationsSystem;
        this.randomService = randomService;
        
        lootGenerator = new LootGenerator();
        inventory = new Inventory();
    }

    public void OpenTombstone(Tombstone tombstone)
    {
        windowService.OpenTombstoneWindow(tombstone);
    }

    public void DigGrave(Tombstone tombstone)
    {
        Console.WriteLine("Digging Grave");
        bool dug = tombstone.Dig();
        if (dug)
        {
            List<ItemData> itemsData = lootGenerator.Generate(tombstone.Data, randomService);
            OnLootSpawn?.Invoke(itemsData, tombstone);
            OnGraveDug?.Invoke(tombstone);
            
            EnemyType enemyType = UndeadGenerator.Generate(tombstone.Data, randomService);
            if (enemyType == EnemyType.Ghost)
            {
                reputationsSystem.RemoveReputation(1);
                Console.WriteLine("Ghost appeared! Reputation reduced by 1!");
            } 
            else if (enemyType == EnemyType.Zombie)
            {
                Console.WriteLine("Zombie appeared! He will be eating you!");
            }
            
            reputationsSystem.RemoveReputation(tombstone.Value);
            windowService.CloseCurrentWindow();
        }
    }

    public void RepairGrave(Tombstone tombstone)
    {
        // TODO: check if we have resources
        bool repaired = tombstone.Repair();
        if (repaired)
        {
            OnGraveRepaired?.Invoke(tombstone);
            reputationsSystem.AddReputation(tombstone.Value);
            windowService.RefreshTombstoneWindow();
        }
    }

    public void PickupItem(ItemData itemData)
    {
        inventory.Add(itemData);
    }

    public void ShowInventory()
    {
        windowService.OpenInventoryWindow(inventory);
    }

    public void ShowMerchant()
    {
        // TODO: make a real merchant
        Inventory merchantInventory = new Inventory();
        merchantInventory.AddMoney(100);
        merchantInventory.Add(lootGenerator.GetRandomItem(randomService));
        merchantInventory.Add(lootGenerator.GetRandomItem(randomService));
        merchantInventory.Add(lootGenerator.GetRandomItem(randomService));
        merchantInventory.Add(lootGenerator.GetRandomItem(randomService));
        merchantInventory.Add(lootGenerator.GetRandomItem(randomService));
        merchantInventory.Add(lootGenerator.GetRandomItem(randomService));
        windowService.OpenTradeWindow(inventory, merchantInventory);
    }
}