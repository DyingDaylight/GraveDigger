using System;
using GraveDigger.Data;
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
    private Window currentWindow;
    
    public TombstoneInfoWindow TombstoneInfoWindow { get;  private set; }
    public InventoryWindow InventoryWindow { get;  private set; }
    public bool IsModalWindow => currentWindow != null;

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
        if (currentWindow != null)
            currentWindow.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (currentWindow != null)
            currentWindow.Draw(spriteBatch);
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

    public void UpdateTombstoneWindow()
    {
        if (TombstoneInfoWindow != null && currentWindow == TombstoneInfoWindow)
        {
            TombstoneInfoWindow.Update();
        }
    }
}