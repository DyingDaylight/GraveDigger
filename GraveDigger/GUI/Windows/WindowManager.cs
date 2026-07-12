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
    
    public bool IsModalWindow => currentWindow != null;
    public bool IsInventoryOpen => currentWindow == InventoryWindow;

    public WindowManager()
    {
        TombstoneInfoWindow = new TombstoneInfoWindow();
        TombstoneInfoWindow.OnCloseButton += CloseCurrentWindow;
        
        InventoryWindow = new InventoryWindow();
        InventoryWindow.OnCloseButton += CloseCurrentWindow;
    }
    
    public void Start()
    {
        TombstoneInfoWindow.Start();
        InventoryWindow.Start();
    }

    public void Update(GameTime gameTime)
    {
        currentWindow?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        currentWindow?.Draw(spriteBatch);
    }
    
    public void OpenTombstoneInfoWindow(Tombstone tombstone)
    {
        TombstoneInfoWindow.SetData(tombstone);
        currentWindow = TombstoneInfoWindow;
    }

    public void OpenInventoryWindow(Inventory inventory)
    {
        InventoryWindow.SetInventory(inventory);
        currentWindow = InventoryWindow;
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