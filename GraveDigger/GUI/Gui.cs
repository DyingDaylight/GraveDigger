using System;
using GraveDigger;
using GraveDigger.Core;
using GraveDigger.GUI.Components;
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
    private readonly GameContext gameContext;
    
    private bool started;
    private Game1.GameState gameState;

    public InteractionTooltip InteractionTooltip { get; private set; }
    public WindowManager WindowManager { get; private set; }
    public MenuUI MenuUi { get; private set; }
    public HUD Hud { get; private set; }
    

    public Gui(GameContext gameContext)
    {
        this.gameContext = gameContext
                           ?? throw new ArgumentNullException(nameof(gameContext));
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
        SpriteManager.AddSprite("Coin", "Images/Icons/Coin");
    }
    
    public void Start()
    {
        // Prevent configuring and subscribing buttons more than once.
        if (started)
            throw new InvalidOperationException("GUI has already been started.");

        started = true;

        WindowManager = new WindowManager();
        MenuUi = new MenuUI(gameContext.ScreenSize);
        InteractionTooltip = new InteractionTooltip(gameContext.CoordinatesConverter);
        Hud = new HUD();

        Hud.Start();
        MenuUi.Start();
        WindowManager.Start();
        InteractionTooltip.Start();
    }

    public void Update(GameTime gameTime)
    {
        if (gameState == Game1.GameState.Menu)
        {
            MenuUi.Update(gameTime);
            return;
        }

        Hud.Update(gameTime);
        InteractionTooltip.Update(gameTime);
        WindowManager.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Hud.Draw(spriteBatch);
        InteractionTooltip.Draw(spriteBatch);
        WindowManager.Draw(spriteBatch);
        
        if (gameState == Game1.GameState.Menu)
            MenuUi.Draw(spriteBatch);
    }

    public void SetGameState(Game1.GameState state)
    {
        gameState = state;
    }

    public void OpenTombstoneWindow(Tombstone tombstone)
    {
        WindowManager.OpenTombstoneInfoWindow(tombstone);
    }

    public void OpenInventoryWindow(Inventory inventory)
    {
        WindowManager.OpenInventoryWindow(inventory);
    }

    public void OpenTradeWindow(Inventory inventory, Inventory inventory1)
    {
        WindowManager.OpenTradeWindow(inventory, inventory1);
    }

    public void CloseCurrentWindow()
    {
        WindowManager.CloseCurrentWindow();
    }

    public bool IsModalWindowOpen()
    {
        return WindowManager.IsModalWindow;
    }

    public void RefreshTombstoneWindow()
    {
        WindowManager.RefreshTombstoneWindow();
    }

    public bool IsInventoryOpen()
    {
        return WindowManager.IsInventoryOpen;
    }
}