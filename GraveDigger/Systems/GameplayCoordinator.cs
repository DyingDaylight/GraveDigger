using System;
using System.Collections.Generic;
using GraveDigger.Characters;
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
    private readonly TimeSystem timeSystem;
    private readonly Inventory inventory;
    private readonly Player player;

    private const int HungerPerDay = 10;
    
    private Merchant? currentMerchant;
    
    public event Action<List<ItemData>, Tombstone> OnLootSpawn;
    public event Action<TradeResult> OnTradeCompleted;

    public event Action<GraveSite> OnGraveChanged;
    public event Action OnMarketClosed;
    public event Action<EnemyType, GraveSite> OnUndeadSpawned;
    
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

        lootGenerator = new LootGenerator();
        
        inventory = InventoryGenerator.CreateInventory(randomService);
    }

    public void RecalculateReputation(IEnumerable<IReputationContributor> contributors)
    {
        reputationSystem.Recalculate(contributors);
    }

    public void DayStarted(int currentDay)
    {
        Console.WriteLine("Day started: " + currentDay);
        player.IncreaseHunger(HungerPerDay);
    }
    
    public void OpenTombstone(GraveSite graveSite)
    {
        bool hasEnoughMoney = inventory.HasEnoughMoney(graveSite.RepairCost);
        windowService.OpenTombstoneWindow(graveSite, hasEnoughMoney);
    }

    public void DigGrave(GraveSite graveSite)
    {
        if (graveSite != null && graveSite.Dig())
        {
            List<ItemData> itemsData = lootGenerator.Generate(graveSite.Tombstone.Data, randomService);
            OnLootSpawn?.Invoke(itemsData, graveSite.Tombstone);
            
            windowService.CloseCurrentWindow();
            OnGraveChanged?.Invoke(graveSite);
            
            EnemyType enemyType = UndeadGenerator.Generate(graveSite.Tombstone.Data, randomService, 
                timeSystem.CurrentDayTime == DayTime.Night);
            if (enemyType == EnemyType.Ghost)
            {
                OnUndeadSpawned?.Invoke(enemyType, graveSite);
            } 
        }
    }

    public void RepairGrave(GraveSite graveSite)
    {
        if (graveSite == null)
            return;
        
        int repairCost = graveSite.RepairCost;
        
        if (!inventory.HasEnoughMoney(repairCost))
            return;
        
        bool success = graveSite.Repair();
        if (!success)
            return;
        
        inventory.SpendMoney(repairCost);
        windowService.RefreshTombstoneWindow(inventory.HasEnoughMoney(graveSite.RepairCost));
        OnGraveChanged?.Invoke(graveSite);
    }

    public void PickupItem(ItemData itemData)
    {
        inventory.Add(itemData);
    }

    public void SellItem(ItemData itemData, int amount)
    {
        if (currentMerchant == null)
            return;
        
        Trade(inventory, currentMerchant.Inventory, itemData, amount);
    }

    public void BuyItem(ItemData itemData, int amount)
    {
        if (currentMerchant == null)
            return;
        
        Trade(currentMerchant.Inventory, inventory, itemData, amount);
    }

    public void UseItem(ItemData itemData, int amount)
    {
        if (itemData is not FoodItemData food)
            return;

        if (amount <= 0)
            return;

        int nutritionAmount = food.Nutrition * amount;

        if (!inventory.Remove(food, amount))
            return;

        player.DecreaseHunger(nutritionAmount);
    }
    
    public void DiscardItem(ItemData itemData, int amount)
    {
        inventory.Remove(itemData, amount);
    }

    public void ShowInventory()
    {
        windowService.OpenInventoryWindow(inventory);
    }

    public void ShowMarket(Merchant merchant)
    {
        if (merchant == null || currentMerchant != null)
            return;
        
        currentMerchant = merchant;
        windowService.OpenTradeWindow(inventory, currentMerchant.Inventory);
        windowService.MarketClosed += CloseMarket;
    }

    private void CloseMarket()
    {
        windowService.MarketClosed -= CloseMarket;
        OnMarketClosed?.Invoke();
        currentMerchant = null;
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

        if (!buyer.HasEnoughMoney(fullPrice))
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