using System;
using GraveDigger.Data;
using GUI.Windows;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.GUI.Windows;

public class WindowManager : IUpdatable, IDrawable
{
    TombstoneInfoWindow tombstoneInfoWindow;
    
    private Window currentWindow;
    
    public bool IsModalWindow => currentWindow != null;
    
    public void Start()
    {
        
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
    
    public void OpenTombstoneInfoWindow(TombstoneData tombstoneData)
    {
        if (tombstoneInfoWindow == null)
            tombstoneInfoWindow = new TombstoneInfoWindow();
        
        tombstoneInfoWindow.SetData(tombstoneData);
        tombstoneInfoWindow.OnDigButton += CloseTombstoneInfoWindow;
        tombstoneInfoWindow.OnRepairButton += CloseTombstoneInfoWindow;
        tombstoneInfoWindow.OnCloseButton += CloseTombstoneInfoWindow;
        currentWindow = tombstoneInfoWindow;
    }

    public void CloseTombstoneInfoWindow()
    {
        if (currentWindow == tombstoneInfoWindow)
            currentWindow = null;
    }
}