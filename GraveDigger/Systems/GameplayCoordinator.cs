using System;
using System.Collections.Generic;
using GraveDigger.Enemies;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Utils;
using Interfaces;

namespace GraveDigger.Systems;

public class GameplayCoordinator : IGameplayActions
{
    private readonly ReputationSystem reputationSystem;
    private readonly IGameWindowService windowService;
    private readonly RandomService randomService;
    private readonly LootGenerator lootGenerator;
    private readonly MerchentProvider merchentProvider;
    private readonly Inventory inventory;
    private readonly Player player;

    public event Action<List<ItemData>, Tombstone> OnLootSpawn;

    public event Action<Tombstone> OnGraveDug;
    public event Action<Tombstone> OnGraveRepaired;
    
    public GameplayCoordinator(
        Player player,
        IGameWindowService windowService, 
        ReputationSystem reputationSystem,
        RandomService randomService)
    {
        this.player = player;
        this.windowService = windowService;
        this.reputationSystem = reputationSystem;
        this.randomService = randomService;
        
        merchentProvider = new MerchentProvider();
        lootGenerator = new LootGenerator();
        inventory = new Inventory();
        // TODO: food for testing purposes
        ItemData food = merchentProvider.GetRandomFood(randomService);
        inventory.Add(food);
        inventory.Add(food);
        inventory.Add(food);
        inventory.Add(food);
        inventory.Add(food);
        inventory.Add(food);
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
                reputationSystem.RemoveReputation(1);
                Console.WriteLine("Ghost appeared! Reputation reduced by 1!");
            } 
            else if (enemyType == EnemyType.Zombie)
            {
                Console.WriteLine("Zombie appeared! He will be eating you!");
            }
            
            reputationSystem.RemoveReputation(tombstone.ReputationValue);
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
            reputationSystem.AddReputation(tombstone.ReputationValue);
            windowService.RefreshTombstoneWindow();
        }
    }

    public void PickupItem(ItemData itemData)
    {
        inventory.Add(itemData);
    }

    public void SellItem(ItemData itemData, int amount)
    {
        throw new NotImplementedException();
    }

    public void BuyItem(ItemData itemData, int amount)
    {
        throw new NotImplementedException();
    }

    public void UseItem(ItemData itemData, int amount)
    {
        if (itemData is not FoodItemData food)
            return;

        if (amount <= 0)
            return;

        int nutritionAmount = food.Nutrition * amount;

        player.DecreaseHunger(nutritionAmount);
        inventory.Remove(food, amount);
    }
    
    public void DiscardItem(ItemData itemData, int amount)
    {
        inventory.Remove(itemData, amount);
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
        merchantInventory.Add(merchentProvider.GetRandomFood(randomService));
        merchantInventory.Add(merchentProvider.GetRandomFood(randomService));
        merchantInventory.Add(merchentProvider.GetRandomFood(randomService));
        merchantInventory.Add(merchentProvider.GetRandomFood(randomService));
        windowService.OpenTradeWindow(inventory, merchantInventory);
    }
}