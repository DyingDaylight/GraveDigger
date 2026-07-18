using System;
using System.Collections.Generic;
using GraveDigger.Enemies;
using GraveDigger.GraveSites;
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
    private readonly TimeSystem timeSystem;
    private readonly Inventory inventory;
    private readonly Player player;

    private const int HungerPerDay = 10;
    
    public event Action<List<ItemData>, Tombstone> OnLootSpawn;
    public event Action<TradeResult> OnTradeCompleted;

    public event Action<GraveSite> OnGraveChanged;
    
    public GameplayCoordinator(
        Player player,
        TimeSystem timeSystem,
        IGameWindowService windowService, 
        ReputationSystem reputationSystem,
        RandomService randomService)
    {
        this.player = player;
        this.timeSystem = timeSystem;
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

    public void RecalculateReputation(IEnumerable<Prop> props)
    {
        reputationSystem.Recalculate(props);
    }

    public void DayStarted(int currentDay)
    {
        Console.WriteLine("Day started: " + currentDay);
        player.IncreaseHunger(HungerPerDay);
    }
    
    public void OpenTombstone(GraveSite graveSite)
    {
        windowService.OpenTombstoneWindow(graveSite);
    }

    public void DigGrave(GraveSite graveSite)
    {
        if (graveSite != null && graveSite.Dig())
        {
            List<ItemData> itemsData = lootGenerator.Generate(graveSite.Tombstone.Data, randomService);
            OnLootSpawn?.Invoke(itemsData, graveSite.Tombstone);
            
            EnemyType enemyType = UndeadGenerator.Generate(graveSite.Tombstone.Data, randomService, 
                timeSystem.CurrentDayTime == DayTime.Night);
            if (enemyType == EnemyType.Ghost)
            {
                Console.WriteLine("Ghost appeared!");
            } 
            windowService.CloseCurrentWindow();
            
            OnGraveChanged?.Invoke(graveSite);
        }
    }

    public void RepairGrave(GraveSite graveSite)
    {
        // TODO: check if we have resources
        if (graveSite != null && graveSite.Repair())
        {
            windowService.RefreshTombstoneWindow();
            OnGraveChanged?.Invoke(graveSite);
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
}