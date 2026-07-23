using System;
using System.Collections.Generic;
using GraveDigger.Characters;
using GraveDigger.Enemies;
using GraveDigger.GraveSites;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Utils;
using GUI;
using Interfaces;

namespace GraveDigger.Systems;

public class GameplayCoordinator
{
    private readonly ReputationSystem reputationSystem;
    private readonly Gui gui;
    private readonly RandomService randomService;
    private readonly LootGenerator lootGenerator;
    private readonly TimeSystem timeSystem;
    private readonly Inventory inventory;
    private readonly Level level;
    
    private Merchant? currentMerchant;
    
    public event Action<string> OnNotificationRequested;
    
    public event Action<TradeResult> OnTradeCompleted;

    public GameplayCoordinator(
        Level level,
        TimeSystem timeSystem,
        Gui gui, 
        ReputationSystem reputationSystem,
        RandomService randomService)
    {
        this.timeSystem = timeSystem;
        this.gui = gui;
        this.reputationSystem = reputationSystem;
        this.randomService = randomService;
        this.level = level;

        lootGenerator = new LootGenerator();
        
        inventory = InventoryGenerator.CreatePlayerInventory(randomService);

        RegisterSubscriptions();
    }

    private void RegisterSubscriptions()
    {
        // Level -> Coordinator
        level.ReputationRecalculationRequested += RecalculateReputation;
        level.ItemPickupRequested += OnItemPickupRequested;
        level.GraveOpenRequested += OpenTombstone;
        level.MarketOpenRequested += ShowMarket;
        
        // GUI -> Coordinator
        gui.WindowManager.TombstoneInfoWindow.OnDigButton += (tombstone) => DigGrave(tombstone.ParentSite);
        gui.WindowManager.TombstoneInfoWindow.OnRepairButton += (tombstone) => RepairGrave(tombstone.ParentSite);
        
        gui.WindowManager.InventoryWindow.UseRequested += UseItem;
        gui.WindowManager.InventoryWindow.DiscardRequested += DiscardItem;

        gui.WindowManager.TradeWindow.SellRequested += SellItem;
        gui.WindowManager.TradeWindow.BuyRequested += BuyItem;
        gui.WindowManager.TradeWindow.UseRequested += UseItem;
        gui.WindowManager.TradeWindow.DiscardRequested += DiscardItem;

        gui.Hud.InventoryRequested += ShowInventory;
        
        // Coordinator -> GUI
        OnNotificationRequested += gui.ShowNotification;
        OnTradeCompleted += gui.ShowTradeResult;

        // TimeSystem -> Level
        timeSystem.DayTimeChanged += level.DayTimeChange;
        timeSystem.DayStarted += level.DayStart;
    }

    public void RecalculateReputation()
    {
        IEnumerable<IReputationContributor> contributors = level.GetReputationContributors();
        reputationSystem.Recalculate(contributors);
    }
    
    public void OpenTombstone(GraveSite graveSite)
    {
        bool hasEnoughMoney = inventory.HasEnoughMoney(graveSite.RepairCost);
        gui.OpenTombstoneWindow(graveSite, hasEnoughMoney);
    }

    public void DigGrave(GraveSite graveSite)
    {
        if (graveSite != null && graveSite.Dig())
        {
            List<ItemData> itemsData = lootGenerator.Generate(graveSite.Tombstone.Data, randomService);
            level.SpawnLoot(itemsData, graveSite.Tombstone);
            
            gui.CloseCurrentWindow();
            level.GraveChanged(graveSite);
            
            EnemyType enemyType = UndeadGenerator.Generate(graveSite.Tombstone.Data, randomService, 
                timeSystem.CurrentDayTime == DayTime.Night);
            if (enemyType != EnemyType.None)
                level.SpawnUndead(enemyType, graveSite);
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
        gui.RefreshTombstoneWindow(inventory.HasEnoughMoney(graveSite.RepairCost));
        level.GraveChanged(graveSite);
    }

    public void OnItemPickupRequested(ItemPickUp itemPickUp)
    {
        ItemData itemData = itemPickUp.ItemData;
        if (!inventory.Add(itemData))
        {
            OnNotificationRequested?.Invoke("Inventory is full.");
            return;
        }

        level.RemovePickup(itemPickUp);
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
        if (amount <= 0)
            return;
        
        switch (itemData)
        {
            case FoodItemData foodItemData:
                int nutritionAmount = foodItemData.Nutrition * amount;
                
                if (!inventory.Remove(foodItemData, amount))
                    return;
                
                level.DecreaseHunger(nutritionAmount);
                break;
            
            case BlueprintItemData blueprintItemData:
                if (!inventory.HasItem(blueprintItemData, amount))
                    return;

                while (amount > 0)
                {
                    if (level.BuildDecoration(blueprintItemData) != true)
                    {
                        OnNotificationRequested?.Invoke(
                            $"There is no place for another {blueprintItemData.Product}.");
                        return;
                    }
                    
                    if (!inventory.Remove(blueprintItemData, 1))
                        return;
                    
                    amount--;
                    OnNotificationRequested?.Invoke(
                        $"{blueprintItemData.Product} built.");
                }
                break;
            
            default:
                break;
        }
    }
    
    public void DiscardItem(ItemData itemData, int amount)
    {
        if (amount <= 0)
            return;
        
        inventory.Remove(itemData, amount);
    }

    public void ShowInventory()
    {
        gui.OpenInventoryWindow(inventory);
    }

    public void ShowMarket(Merchant merchant)
    {
        if (merchant == null || currentMerchant != null)
            return;
        
        currentMerchant = merchant;
        gui.MarketClosed += CloseMarket;
        gui.OpenTradeWindow(inventory, currentMerchant.Inventory);
    }

    private void CloseMarket()
    {
        gui.MarketClosed -= CloseMarket;
        level.MarketClosed();
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
        buyer.SpendMoney(fullPrice);

        buyer.Add(itemData, amount);
        seller.AddMoney(fullPrice);
        
        AudioManager.Instance.PlaySFX("coins");
        OnTradeCompleted?.Invoke(result);
    }
}