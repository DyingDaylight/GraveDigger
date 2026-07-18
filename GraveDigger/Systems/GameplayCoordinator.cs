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
    private readonly MerchantProvider merchantProvider;
    private readonly Inventory merchantInventory;
    private readonly Inventory inventory;
    private readonly Player player;
    
    private Level level;

    public event Action<List<ItemData>, Tombstone> OnLootSpawn;
    public event Action<TradeResult> OnTradeCompleted;

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
        
        merchantProvider = new MerchantProvider();
        lootGenerator = new LootGenerator();
        merchantInventory = new Inventory();
        // TODO: make a real merchant
        merchantInventory.AddMoney(100);
        merchantInventory.Add(merchantProvider.GetRandomFood(randomService));
        merchantInventory.Add(merchantProvider.GetRandomFood(randomService));
        merchantInventory.Add(merchantProvider.GetRandomFood(randomService));
        merchantInventory.Add(merchantProvider.GetRandomFood(randomService));
        
        inventory = new Inventory();
        // TODO: food for testing purposes
        ItemData food = merchantProvider.GetRandomFood(randomService);
        inventory.Add(food);
        inventory.Add(food);
        inventory.Add(food);
        inventory.Add(food);
        inventory.Add(food);
        inventory.Add(food);
    }
    
    public void CalculateInitialReputation(List<Prop> props)
    {
        reputationSystem.Calculate(props);
    }

    public void OpenTombstone(Tombstone tombstone)
    {
        if (level == null) return;
        
        var graveSite = level.GetGraveSiteByTombstone(tombstone);
        windowService.OpenTombstoneWindow(tombstone, graveSite);    
    }

    public void DigGrave(Tombstone tombstone)
    {
        if (level == null) return;
        
        Console.WriteLine("Digging Grave");
        var graveSite = level.GetGraveSiteByTombstone(tombstone);
        if (graveSite != null && graveSite.Dig())
            
        {
            List<ItemData> itemsData = lootGenerator.Generate(tombstone.Data, randomService);
            OnLootSpawn?.Invoke(itemsData, tombstone);
            OnGraveDug?.Invoke(tombstone);
            
            EnemyType enemyType = UndeadGenerator.Generate(tombstone.Data, randomService);
            if (enemyType == EnemyType.Ghost)
            {
                reputationSystem.ChangeReputation(1);
                Console.WriteLine("Ghost appeared! Reputation reduced by 1!");
            } 
            else if (enemyType == EnemyType.Zombie)
            {
                Console.WriteLine("Zombie appeared! He will be eating you!");
            }
            
            reputationSystem.ChangeReputation(graveSite.GetReputationValue());
            windowService.CloseCurrentWindow();
        }
    }

    public void RepairGrave(Tombstone tombstone)
    {
        if (level == null) return;
        
        var graveSite = level.GetGraveSiteByTombstone(tombstone);
        // TODO: check if we have resources
        if (graveSite != null && graveSite.Repair())
        {
            OnGraveRepaired?.Invoke(tombstone);
            reputationSystem.ChangeReputation(graveSite.GetReputationValue());
            windowService.RefreshTombstoneWindow();
        }
    }

    public void PickupItem(ItemData itemData)
    {
        inventory.Add(itemData);
    }

    public void SellItem(ItemData itemData, int amount)
    {
        Trade(inventory, merchantInventory, itemData, amount);
    }

    public void BuyItem(ItemData itemData, int amount)
    {
        Trade(merchantInventory, inventory, itemData, amount);
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

    public void ShowMarket()
    {
        windowService.OpenTradeWindow(inventory, merchantInventory);
    }
    
    private TradeResult ValidateTrade(Inventory seller, Inventory buyer,
        ItemData itemData, int amount)
    {
        if (itemData == null)
            return TradeResult.ItemNotFound;

        if (amount <= 0)
            return TradeResult.InvalidQuantity;
        
        if (!seller.HasItem(itemData, amount))
            return TradeResult.InvalidQuantity;

        int fullPrice = itemData.Price * amount;

        if (buyer.Money < fullPrice)
            return TradeResult.NotEnoughMoney;
        
        if (!buyer.CanAdd(itemData, amount))
            return TradeResult.NotEnoughInventorySpace;

        return TradeResult.Success;
    }
    
    private void Trade(Inventory seller, Inventory buyer, ItemData itemData, int amount)
    {
        TradeResult result = ValidateTrade(seller, buyer,
            itemData, amount);

        if (result != TradeResult.Success)
        {
            AudioManager.Instance.PlaySFX("chest-close");
            OnTradeCompleted?.Invoke(result);
            return;
        }

        int fullPrice = itemData.Price * amount;

        seller.Remove(itemData, amount);
        seller.AddMoney(fullPrice);
        
        buyer.Add(itemData, amount);
        buyer.SpendMoney(fullPrice);

        AudioManager.Instance.PlaySFX("coins");
        OnTradeCompleted?.Invoke(result);
    }
    
    public void SetLevel(Level level)
    {
        this.level = level;
    }
}