using System;
using System.Collections.Generic;
using GraveDigger;
using GraveDigger.Data;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Windows;
using GraveDigger.Items;
using GraveDigger.Props;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GUI;

public class Gui: IUpdatable, IDrawable, IGameWindowService
{
    public InteractionTooltip interactionTooltip;
    public WindowManager WindowManager { get; private set; }
    public MenuUI MenuUi;
    public HUD hud;
    
    private bool started = false;
    private Game1.GameState gameState;

    private GameContext gameContext;

    public Gui(GameContext gameContext)
    {
        this.gameContext = gameContext;
    }
    
    public void LoadContent(ContentManager content)
    {
        GUIResources.LoadContent(content);
        
        SpriteManager.AddSprite("ButtonNormal", "Images/GUI/ButtonNormal");
        SpriteManager.AddSprite("ButtonHover", "Images/GUI/ButtonHover");
        SpriteManager.AddSprite("ButtonPressed", "Images/GUI/ButtonPressed");
        SpriteManager.AddSprite("CloseButtonNormal", "Images/GUI/CloseButtonNormal");
        SpriteManager.AddSprite("CloseButtonHover", "Images/GUI/CloseButtonHover");
        SpriteManager.AddSprite("CloseButtonPressed", "Images/GUI/CloseButtonPressed");
    }
    
    public void Start()
    {
        // Prevent configuring and subscribing buttons more than once.
        if (started)
            throw new InvalidOperationException("GUI has already been started.");

        started = true;

        WindowManager = new WindowManager();
        MenuUi = new MenuUI(gameContext.ScreenSize);
        interactionTooltip = new InteractionTooltip(gameContext.CoordinatesConverter);
        hud = new HUD();

        hud.Start();
        MenuUi.Start();
        WindowManager.Start();
        interactionTooltip.Start();
    }

    public void Update(GameTime gameTime)
    {
        if (gameState == Game1.GameState.Menu)
            MenuUi.Update(gameTime);
        
        interactionTooltip.Update(gameTime);
        WindowManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        interactionTooltip.Draw(spriteBatch);
        WindowManager.Draw(spriteBatch);
        if (gameState == Game1.GameState.Menu)
            MenuUi.Draw(spriteBatch);
    }

    public void SetGameState(Game1.GameState state)
    {
        gameState = state;
    }

    public void OpenTombstoneWindow(Tombstone tombstoneData)
    {
        WindowManager.OpenTombstoneInfoWindow(tombstoneData);
    }

    public void OpenInventoryWindow(Inventory inventory)
    {
        WindowManager.OpenInventoryWindow(inventory);
    }

    public void CloseCurrentWindow()
    {
        WindowManager.CloseCurrentWindow();
    }

    public bool IsModalWindowOpen()
    {
        return WindowManager.IsModalWindow;
    }

    public void UpdateTombstoneWindow()
    {
        WindowManager.UpdateTombstoneWindow();
    }

    public bool IsInventoryOpen()
    {
        return false;
    }
}