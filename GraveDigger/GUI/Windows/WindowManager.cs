using System;
using GraveDigger.Core;
using GraveDigger.GraveSites;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Systems;
using GUI.Windows;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.GUI.Windows;

public class WindowManager : IUpdatable, IDrawable
{ 
    private Window? currentWindow;

    public GravePreparationWindow GravePreparationWindow { get; }
    public TombstoneInfoWindow TombstoneInfoWindow { get; }
    public InventoryWindow InventoryWindow { get; }
    public GameOverWindow GameOverWindow { get; }
    public TradeWindow TradeWindow { get; }
    
    public bool IsModalWindow => currentWindow != null;
    public bool IsInventoryOpen => currentWindow == InventoryWindow;

    public WindowManager(GameContext gameContext)
    {
        Rectangle bounds = new Rectangle(0, 0, (int)gameContext.ScreenSize.X, (int) gameContext.ScreenSize.Y);
        
        GravePreparationWindow = new GravePreparationWindow(bounds);
        GravePreparationWindow.OnCloseButton += CloseCurrentWindow;
        
        TombstoneInfoWindow = new TombstoneInfoWindow(bounds);
        TombstoneInfoWindow.OnCloseButton += CloseCurrentWindow;
        
        InventoryWindow = new InventoryWindow(bounds);
        InventoryWindow.OnCloseButton += CloseCurrentWindow;
        
        TradeWindow = new TradeWindow(bounds);
        TradeWindow.OnCloseButton += CloseCurrentWindow;
        
        GameOverWindow = new GameOverWindow(bounds);
    }
    
    public void Start()
    {
        GravePreparationWindow.Start();
        TombstoneInfoWindow.Start();
        InventoryWindow.Start();
        GameOverWindow.Start();
        TradeWindow.Start();
    }

    public void Update(GameTime gameTime)
    {
        currentWindow?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        currentWindow?.Draw(spriteBatch);
    }

    public void OpenGravePreparationWindow(GraveSite graveSite)
    {
        if (IsModalWindow)
            return;
        
        GravePreparationWindow.SetData(graveSite);
        currentWindow = GravePreparationWindow;
    }
    
    public void OpenTombstoneInfoWindow(GraveSite graveSite, bool hasEnoughMoney)
    {
        if (IsModalWindow)
            return;
        
        TombstoneInfoWindow.SetData(graveSite, hasEnoughMoney);
        currentWindow = TombstoneInfoWindow;
    }

    public void OpenInventoryWindow(Inventory inventory)
    {
        if (IsModalWindow)
            return;
        
        InventoryWindow.SetInventory(inventory);
        currentWindow = InventoryWindow;
    }
    
    public void OpenTradeWindow(Inventory playerInventory, Inventory merchantInventory)
    {
        if (IsModalWindow)
            return;
        
        TradeWindow.SetInventories(playerInventory, merchantInventory);
        currentWindow = TradeWindow;
    }

    public void CloseCurrentWindow()
    {
        currentWindow = null;
    }

    public void RefreshTombstoneWindow(bool hasEnoughMoney)
    {
        if (currentWindow != null && currentWindow == TombstoneInfoWindow)
            TombstoneInfoWindow.Refresh(hasEnoughMoney);
    }

    public void ShowTradeResult(TradeResult result)
    {
        if (currentWindow != null && currentWindow == TradeWindow)
            TradeWindow.ShowTradeResult(result);
    }

    public void OpenGameOverWindow(GameResult result)
    {
        GameOverWindow.SetResult(result);
        currentWindow = GameOverWindow;
    }
}