using GraveDigger.Core;
using GraveDigger.GraveSites;
using GraveDigger.Items;
using GraveDigger.Props;
using GUI.Windows;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.GUI.Windows;

public class WindowManager : IUpdatable, IDrawable
{ 
    private Window? currentWindow;
    
    public TombstoneInfoWindow TombstoneInfoWindow { get; }
    public InventoryWindow InventoryWindow { get; }
    public TradeWindow TradeWindow { get; }
    
    public bool IsModalWindow => currentWindow != null;
    public bool IsInventoryOpen => currentWindow == InventoryWindow;

    public WindowManager(GameContext gameContext)
    {
        Rectangle bounds = new Rectangle(0, 0, (int)gameContext.ScreenSize.X, (int) gameContext.ScreenSize.Y);
        
        TombstoneInfoWindow = new TombstoneInfoWindow(bounds);
        TombstoneInfoWindow.OnCloseButton += CloseCurrentWindow;
        
        InventoryWindow = new InventoryWindow(bounds);
        InventoryWindow.OnCloseButton += CloseCurrentWindow;
        
        TradeWindow = new TradeWindow(bounds);
        TradeWindow.OnCloseButton += CloseCurrentWindow;
    }
    
    public void Start()
    {
        TombstoneInfoWindow.Start();
        InventoryWindow.Start();
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
    
    public void OpenTombstoneInfoWindow(GraveSite graveSite)
    {
        TombstoneInfoWindow.SetData(graveSite.Tombstone, graveSite);
        currentWindow = TombstoneInfoWindow;
    }

    public void OpenInventoryWindow(Inventory inventory)
    {
        InventoryWindow.SetInventory(inventory);
        currentWindow = InventoryWindow;
    }
    
    public void OpenTradeWindow(Inventory inventory, Inventory inventory1)
    {
        TradeWindow.SetInventories(inventory, inventory1);
        currentWindow = TradeWindow;
    }

    public void CloseCurrentWindow()
    {
        currentWindow = null;
    }

    public void RefreshTombstoneWindow()
    {
        if (currentWindow == TombstoneInfoWindow)
            TombstoneInfoWindow?.Refresh();
    }
}