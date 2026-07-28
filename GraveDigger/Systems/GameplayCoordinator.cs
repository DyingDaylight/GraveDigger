using System;
using System.Collections.Generic;
using System.Linq;
using GraveDigger.Characters;
using GraveDigger.Enemies;
using GraveDigger.GraveSites;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Utils;
using GUI;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Systems;

public class GameplayCoordinator : IUpdatable
{
    private readonly ReputationSystem reputationSystem;
    private readonly RandomService randomService;
    private readonly TimeSystem timeSystem;
    private readonly Inventory inventory;
    private readonly Level level;
    private readonly Gui gui;
    
    private Merchant currentMerchant;
    
    public event Action<TradeResult> TradeCompleted;
    public event Action<string> NotificationRequested;

    
    public GameplayCoordinator(
        Gui gui, 
        Level level,
        TimeSystem timeSystem,
        RandomService randomService)
    {
        this.gui = gui;
        this.level = level;
        this.timeSystem = timeSystem;
        this.randomService = randomService;

        reputationSystem = new ReputationSystem();

        inventory = InventoryGenerator.CreatePlayerInventory(randomService);
    }

    public void Start()
    {
        timeSystem.Start();
        RegisterSubscriptions();
        RecalculateReputation();
    }

    public void Update(GameTime gameTime)
    {
        timeSystem.Update(gameTime);
    }

    public void ToggleInventory()
    {
        if (gui.IsInventoryOpen())
        {
            gui.CloseCurrentWindow();
            return;
        }

        if (!gui.IsModalWindowOpen())
            ShowInventory();
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
        
        if (currentMerchant == null)
            return;

        currentMerchant = null;
        level.MarketClosed();
    }

    private void ShowInventory()
    {
        gui.OpenInventoryWindow(inventory);
    }
    
    private void RecalculateReputation()
    {
        IEnumerable<IReputationContributor> contributors = level.GetReputationContributors();
        reputationSystem.Recalculate(contributors);
    }
    
    private void OpenTombstone(GraveSite graveSite)
    {
        bool hasEnoughMoney = inventory.HasEnoughMoney(graveSite.RepairCost);
        gui.OpenTombstoneWindow(graveSite, hasEnoughMoney);
    }
    
    private void DigRequested(Tombstone tombstone)
    {
        DigGrave(tombstone.ParentSite);
    }

    private void RepairRequested(Tombstone tombstone)
    {
        RepairGrave(tombstone.ParentSite);
    }

    private void DigGrave(GraveSite graveSite)
    {
        if (graveSite == null)
            return;

        bool isDug = graveSite.Dig();
        if (!isDug)
            return;
        
        List<ItemData> itemsData = LootGenerator.Generate(graveSite.Tombstone.Data, randomService);
        level.SpawnLoot(itemsData, graveSite.Tombstone);
        
        gui.CloseCurrentWindow();
        RecalculateReputation();
        
        EnemyType enemyType = UndeadGenerator.Generate(graveSite.Tombstone.Data, randomService, 
            timeSystem.CurrentDayTime == DayTime.Night);
        if (enemyType != EnemyType.None)
            level.SpawnUndead(enemyType, graveSite);
    }

    private void RepairGrave(GraveSite graveSite)
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
        RecalculateReputation();
    }

    private void OnItemPickupRequested(ItemPickUp itemPickUp)
    {
        ItemData itemData = itemPickUp.ItemData;
        if (!inventory.Add(itemData))
        {
            NotificationRequested?.Invoke("Inventory is full.");
            return;
        }

        level.RemovePickup(itemPickUp);
    }

    private void SellItem(ItemData itemData, int amount)
    {
        if (currentMerchant == null)
            return;
        
        Trade(inventory, currentMerchant.Inventory, itemData, amount);
    }

    private void BuyItem(ItemData itemData, int amount)
    {
        if (currentMerchant == null)
            return;
        
        Trade(currentMerchant.Inventory, inventory, itemData, amount);
    }

    private void UseItem(ItemData itemData, int amount)
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
                        NotificationRequested?.Invoke(
                            $"There is no place for another {blueprintItemData.Product}.");
                        return;
                    }
                    
                    if (!inventory.Remove(blueprintItemData))
                        return;
                    
                    amount--;
                    NotificationRequested?.Invoke(
                        $"{blueprintItemData.Product} built.");
                    RecalculateReputation();
                }
                break;
        }
    }
    
    private void DiscardItem(ItemData itemData, int amount)
    {
        if (amount <= 0)
            return;
        
        inventory.Remove(itemData, amount);
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
            TradeCompleted?.Invoke(result);
            return;
        }

        int fullPrice = itemData.Price * amount;

        seller.Remove(itemData, amount);
        buyer.SpendMoney(fullPrice);

        buyer.Add(itemData, amount);
        seller.AddMoney(fullPrice);
        
        AudioManager.Instance.PlaySFX("coins");
        TradeCompleted?.Invoke(result);
    }
    
    private void RegisterSubscriptions()
    {
        // Level -> Coordinator
        level.ReputationRecalculationRequested += RecalculateReputation;
        level.ItemPickupRequested += OnItemPickupRequested;
        level.GraveOpenRequested += OpenTombstone;
        level.MarketOpenRequested += ShowMarket;
        
        // GUI -> Coordinator
        gui.WindowManager.TombstoneInfoWindow.DigButtonPressed += DigRequested;
        gui.WindowManager.TombstoneInfoWindow.RepairButtonPressed += RepairRequested;
        
        gui.WindowManager.InventoryWindow.UseRequested += UseItem;
        gui.WindowManager.InventoryWindow.DiscardRequested += DiscardItem;

        gui.WindowManager.TradeWindow.SellRequested += SellItem;
        gui.WindowManager.TradeWindow.BuyRequested += BuyItem;
        gui.WindowManager.TradeWindow.UseRequested += UseItem;
        gui.WindowManager.TradeWindow.DiscardRequested += DiscardItem;

        gui.Hud.InventoryRequested += ShowInventory;
        
        // Coordinator -> GUI
        NotificationRequested += gui.ShowNotification;
        TradeCompleted += gui.ShowTradeResult;

        // TimeSystem -> Level
        timeSystem.DayTimeChanged += level.DayTimeChange;
        timeSystem.DayStarted += level.DayStart;
        
        // ReputationSystem -> GUI
        reputationSystem.ReputationChanged += gui.Hud.UpdateReputation;
    }
}