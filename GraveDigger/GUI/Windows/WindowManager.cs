using System;
using GraveDigger.Data;
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
    public bool IsModalWindow => currentWindow != null;

    public WindowManager()
    {
        TombstoneInfoWindow = new TombstoneInfoWindow();
        TombstoneInfoWindow.OnCloseButton += CloseCurrentWindow;
    }
    
    public void Start()
    {
        TombstoneInfoWindow.Start();
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

    public void CloseCurrentWindow()
    {
        currentWindow = null;
    }
}