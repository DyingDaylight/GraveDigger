using System;
using GraveDigger;
using GraveDigger.Core;
using GraveDigger.GUI.Components;
using GraveDigger.GUI.Windows;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Systems;
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
    private GameState gameState;

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
        
        SpriteManager.AddSprite("MainMenuBackground", "Images/GUI/mainmenu");
        SpriteManager.AddSprite("ButtonMainMenu", "Images/GUI/button_mainmenu");
        SpriteManager.AddSprite("ButtonHover", "Images/GUI/buttonhover_mainmenu");
        SpriteManager.AddSprite("ButtonPressed", "Images/GUI/buttonpressed_mainmenu");
        SpriteManager.AddSprite("background", "Images/GUI/background");
        SpriteManager.AddSprite("slot", "Images/GUI/slot");
        SpriteManager.AddSprite("slothover", "Images/GUI/slothover");
        SpriteManager.AddSprite("ButtonDisabled", "Images/GUI/button_disabled");
        SpriteManager.AddSprite("InventoryButtonNormal", "Images/GUI/inventory_button_normal");
        SpriteManager.AddSprite("InventoryButtonHover", "Images/GUI/inventory_button_hover");
        SpriteManager.AddSprite("InventoryButtonPressed", "Images/GUI/inventory_button_pressed");
        SpriteManager.AddSprite("InventoryButtonDisabled", "Images/GUI/inventory_button_disabled");

        SpriteManager.AddSprite("ReputationBadIcon", "Images/GUI/reputation_bad");
        SpriteManager.AddSprite("ReputationGoodIcon", "Images/GUI/reputation_good");
        SpriteManager.AddSprite("ReputationLineIcon", "Images/GUI/reputation_line");
        SpriteManager.AddSprite("ReputationSliderIcon", "Images/GUI/reputation_slider");

        SpriteManager.AddSprite("Coin", "Images/Icons/Coin");
    }
    
    public void Start()
    {
        // Prevent configuring and subscribing buttons more than once.
        if (started)
            throw new InvalidOperationException("GUI has already been started.");

        started = true;

        WindowManager = new WindowManager(gameContext);
        MenuUi = new MenuUI(gameContext.ScreenSize);
        InteractionTooltip = new InteractionTooltip(gameContext.CoordinatesConverter);
        Hud = new HUD(gameContext);

        Hud.Start();
        MenuUi.Start();
        WindowManager.Start();
        InteractionTooltip.Start();
    }

    public void Update(GameTime gameTime)
    {
        if (gameState == GameState.Menu)
        {
            MenuUi.Update(gameTime);
            return;
        }

        WindowManager.Update(gameTime);
        if (WindowManager.IsModalWindow)
            return;
        
        Hud.Update(gameTime);
        InteractionTooltip.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        InteractionTooltip.Draw(spriteBatch);
        WindowManager.Draw(spriteBatch);

        if (gameState == GameState.Menu)
            MenuUi.Draw(spriteBatch);
        else
            Hud.Draw(spriteBatch);
    }

    public void SetGameState(GraveDigger.GameState state) 
    {
        this.gameState = state; 
    }

    public void OpenTombstoneWindow(Tombstone tombstone, GraveDigger.GraveSites.GraveSite graveSite)
    {
        WindowManager.OpenTombstoneInfoWindow(tombstone, graveSite);
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

    public void ShowTradeResult(TradeResult result)
    {
        if (WindowManager.TradeWindow != null && WindowManager.TradeWindow.Visible)
            WindowManager.TradeWindow.ShowTradeResult(result);
    }

    public bool IsInventoryOpen()
    {
        return WindowManager.IsInventoryOpen;
    }
}