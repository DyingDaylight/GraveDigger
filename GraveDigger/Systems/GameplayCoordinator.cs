using System;
using System.Collections.Generic;
using System.Linq;
using GraveDigger.Characters;
using GraveDigger.Data;
using GraveDigger.Enemies;
using GraveDigger.GraveSites;
using GraveDigger.GUI.Windows;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Utils;
using GUI;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.Systems;

public class GameplayCoordinator : IUpdatable
{
    private readonly ReputationSystem reputationSystem;
    private readonly GraveDecaySystem decaySystem;
    private readonly RandomService randomService;
    private readonly TimeSystem timeSystem;
    private readonly Inventory inventory;
    private readonly Level level;
    private readonly Gui gui;
    
    private Merchant currentMerchant;
    private int previousReputation;
    private bool reputationInitialized;
    private bool gameEnded;
    
    private KeyboardState previousKeyboardState;
    private int debugReputationOffset = 0;
    
    public event Action<TradeResult> TradeCompleted;
    public event Action<string> NotificationRequested;
    public event Action<GameResult> GameEnded;
    
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

        decaySystem = new GraveDecaySystem(randomService);
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
        if (!level.IsBlackoutRunning)
            timeSystem.Update(gameTime);
        
#if DEBUG
        HandleDebugInput();
#endif
    
        CheckNearDeathWarning();
        
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
        #if DEBUG
        debugReputationOffset = 0;
        #endif
        
        IEnumerable<IReputationContributor> contributors = level.GetReputationContributors();
        reputationSystem.Recalculate(contributors);
    }
    
    private void InteractWithGraveSite(GraveSite graveSite)
    {
        switch (graveSite.Status)
        {
            case GraveSiteStatus.Locked:
                OpenGravePreparation(graveSite);
                break;

            case GraveSiteStatus.Prepared:
                gui.ShowNotification("This grave plot is waiting for its guest.");
                break;

            case GraveSiteStatus.Occupied:
                OpenTombstone(graveSite);
                break;
        }
    }

    private void OpenGravePreparation(GraveSite graveSite)
    {
        gui.OpenGravePreparationWindow(graveSite);
    }
    
    private void OnGraveOccupied(GraveSite graveSite)
    {
        inventory.AddMoney(GraveSiteGenerator.GetRewardForBurial(graveSite.Tombstone.Data.Wealth));
        RecalculateReputation();
        NotificationRequested?.Invoke("A new burial has arrived.");
    }

    private void OpenTombstone(GraveSite graveSite)
    {
        bool hasEnoughMoney = inventory.HasEnoughMoney(graveSite.RepairCost);
        gui.OpenTombstoneWindow(graveSite, hasEnoughMoney);
    }
    
    private void PrepareRequested(GraveSite graveSite)
    {
        if (!graveSite.CanPrepare)
            return;
        
        RunTimedAction(Game1.DiggingTime, () => PrepareGrave(graveSite));
    }
    
    private void DigRequested(Tombstone tombstone)
    {
        GraveSite graveSite = tombstone.ParentSite;
        if (!graveSite.CanDig)
            return;
        
        RunTimedAction(Game1.DiggingTime, () => DigGrave(graveSite));
    }

    private void RepairRequested(Tombstone tombstone)
    {
        RepairGrave(tombstone.ParentSite);
    }

    private void RunTimedAction(int seconds, Action action)
    {
        gui.CloseCurrentWindow();

        bool started = level.RunBlackout(() =>
        {
            AudioManager.Instance.StopSFX("shovel");
            timeSystem.AdvanceTime(seconds);
            if (gameEnded)
                return;
            
            action();
        });

        if (started)
            AudioManager.Instance.PlaySFX("shovel");
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
        
        RecalculateReputation();
        
        EnemyType enemyType = UndeadGenerator.Generate(graveSite.Tombstone.Data, randomService, 
            timeSystem.CurrentDayTime == DayTime.Night);
        if (enemyType != EnemyType.None)
        {
            level.SpawnUndead(enemyType, graveSite);
        }
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

    private void PrepareGrave(GraveSite graveSite)
    {
        if (!graveSite.Prepare())
            return;

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
        level.GraveInteractionRequested += InteractWithGraveSite;
        level.MarketOpenRequested += ShowMarket;
        level.GraveOccupied += OnGraveOccupied;
        
        level.Player.HungerChanged += OnHungerChanged;
        
        // GUI -> Coordinator
        gui.WindowManager.GravePreparationWindow.PrepareButtonPressed += PrepareRequested;
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
        
        // TimeSystem -> Coordinator
        timeSystem.DayStarted += OnDayStart;
        timeSystem.TimeUpdated += OnTimeUpdated;
        
        // ReputationSystem -> Coordinator
        reputationSystem.ReputationChanged += OnReputationChanged;
    }

    private void OnHungerChanged(int value, int min, int max)
    {
        gui.Hud.UpdateHunger(value, min, max);
        
        CheckGameOver();
    }

    private void OnReputationChanged(int value, int min, int max)
    {
        gui.Hud.UpdateReputation(value, min, max);

        if (!reputationInitialized)
        {
            previousReputation = value;
            reputationInitialized = true;
            return;
        }

        int delta = value - previousReputation;
        previousReputation = value;

        if (delta == 0)
            return;
        
        NotificationRequested?.Invoke($"Reputation {(delta > 0 ? "+" : "")}{delta}");
        AudioManager.Instance.PlaySFX("ding");

        CheckGameOver();
    }
    
    private void OnDayStart(int day)
    {
        decaySystem.DecayGraves(level.GraveSites);
        TryOccupyGrave();
    }

    private void TryOccupyGrave()
    {
        float chance = BurialChanceCalculator.Calculate(reputationSystem.Value);

        if (!randomService.Chance(chance))
            return;
        
        level.OccupyPreparedGraveSite();
    }
    
    private void CheckGameOver()
    {
        if (reputationSystem.Value <= ReputationSystem.MinValue)
        {
            EndGame(GameResult.LoseReputation);
            return;
        }

        if (level.Player.IsStarving)
        {
            EndGame(GameResult.LoseHunger);
            return;
        }

        if (reputationSystem.Value >= ReputationSystem.MaxValue)
        {
            EndGame(GameResult.Win);
        }
    }
    
    private void CheckNearDeathWarning()
    {
        if (gameEnded)
        {
            gui.HideNearDeathWarning();
            return;
        }
    
        int currentRep = GetEffectiveReputation();

        bool isLowReputation = currentRep <= -70;
        bool isStarving = level.Player.Hunger >= 75;
    
        if (!isLowReputation && !isStarving)
        {
            gui.HideNearDeathWarning();
            return;
        }

        if (isLowReputation && isStarving)
        {
            gui.ShowNearDeathWarning("CRITICAL WARNING: You are starving and your reputation is critically low!");
        }
        else if (isLowReputation)
        {
            gui.ShowNearDeathWarning("WARNING: Reputation is critically low! You are about to lose.");
        }
        else if (isStarving)
        {
            gui.ShowNearDeathWarning("WARNING: You are starving! Find food before you die.");
        }
    }
    
    // to test popup logic
#if DEBUG
    private void HandleDebugInput()
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();

        // [H] - to add hunger
        if (currentKeyboardState.IsKeyDown(Keys.H) && previousKeyboardState.IsKeyUp(Keys.H))
        {
            level.Player.IncreaseHunger(5);
            NotificationRequested?.Invoke("DEBUG: Hunger +5");
        }

        // [R] - to decrease reputation
        if (currentKeyboardState.IsKeyDown(Keys.R) && previousKeyboardState.IsKeyUp(Keys.R))
        {
            debugReputationOffset -= 30;
            OnReputationChanged(GetEffectiveReputation(), ReputationSystem.MinValue, ReputationSystem.MaxValue);
        }
        
        previousKeyboardState = currentKeyboardState;
    }
#endif
    
    private int GetEffectiveReputation()
    {
        return Math.Clamp(reputationSystem.Value + debugReputationOffset, ReputationSystem.MinValue, ReputationSystem.MaxValue);
    }

    private void EndGame(GameResult result)
    {
        if (gameEnded)
            return;

        gameEnded = true;
        gui.HideNearDeathWarning();
        GameEnded?.Invoke(result);
    }
    
    private void OnTimeUpdated(float progress)
    {
        gui.Hud.UpdateDayTime(progress);
    }
}