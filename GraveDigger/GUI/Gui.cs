using System;
using GraveDigger;
using GraveDigger.Core;
using GraveDigger.GraveSites;
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

public class Gui: IUpdatable, IDrawable
{
    private readonly GameContext gameContext;
    private NotificationPopup notificationPopup;

    private bool started;
    private GameState gameState;

    public InteractionTooltip InteractionTooltip { get; private set; }
    public WindowManager WindowManager { get; private set; }
    public MenuUI MenuUi { get; private set; }
    public HUD Hud { get; private set; }
    
    public event Action MarketClosed;
    

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
        SpriteManager.AddSprite("DayIcon", "Images/GUI/day");
        SpriteManager.AddSprite("NightIcon", "Images/GUI/night");

        SpriteManager.AddSprite("Coin", "Images/Icons/Coin");
        SpriteManager.AddSprite("Hunger", "Images/Gui/hunger");
        
        SpriteManager.AddSprite("hungerResult1", "Images/Results/HungerResult1");
        SpriteManager.AddSprite("hungerResult2", "Images/Results/HungerResult2");
        SpriteManager.AddSprite("hungerResult3", "Images/Results/HungerResult3");
        
        SpriteManager.AddSprite("reputationResult1", "Images/Results/ReputationResult1");
        SpriteManager.AddSprite("reputationResult2", "Images/Results/ReputationResult2");
        SpriteManager.AddSprite("reputationResult3", "Images/Results/ReputationResult3");
        
        SpriteManager.AddSprite("winResult1", "Images/Results/WinResult1");
        SpriteManager.AddSprite("winResult2", "Images/Results/WinResult2");
        SpriteManager.AddSprite("winResult3", "Images/Results/WinResult3");
    }
    
    public void Start()
    {
        // Prevent configuring and subscribing buttons more than once.
        if (started)
            throw new InvalidOperationException("GUI has already been started.");

        notificationPopup = new NotificationPopup(new Rectangle(0, 0, 
            (int) gameContext.ScreenSize.X, 
            (int) gameContext.ScreenSize.Y));
        WindowManager = new WindowManager(gameContext);
        MenuUi = new MenuUI(gameContext.ScreenSize);
        InteractionTooltip = new InteractionTooltip(gameContext.CoordinatesConverter);
        Hud = new HUD(gameContext);

        Hud.Start();
        MenuUi.Start();
        WindowManager.Start();
        notificationPopup.Start();
        InteractionTooltip.Start();

        WindowManager.TradeWindow.OnCloseButton += HandleMarketClosed;
        
        started = true;
    }

    public void Update(GameTime gameTime)
    {
        if (gameState == GameState.Menu)
        {
            MenuUi.Update(gameTime);
            return;
        }

        notificationPopup.Update(gameTime);
        WindowManager.Update(gameTime);
        if (WindowManager.IsModalWindow)
            return;
        
        Hud.Update(gameTime);
        InteractionTooltip.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (gameState == GameState.Menu)
        {
            MenuUi.Draw(spriteBatch);
            return;
        }

        Hud.Draw(spriteBatch);
        WindowManager.Draw(spriteBatch);
        notificationPopup.Draw(spriteBatch);
        
        if (gameState == GameState.Playing)
            InteractionTooltip.Draw(spriteBatch);
    }

    public void SetGameState(GameState state) 
    {
        gameState = state; 
    }

    public void OpenGravePreparationWindow(GraveSite graveSite)
    {
        WindowManager.OpenGravePreparationWindow(graveSite);
    }
    
    public void OpenTombstoneWindow(GraveSite graveSite, bool hasEnoughMoney)
    {
        WindowManager.OpenTombstoneInfoWindow(graveSite, hasEnoughMoney);
    }

    public void OpenInventoryWindow(Inventory inventory)
    {
        WindowManager.OpenInventoryWindow(inventory);
    }

    public void OpenTradeWindow(Inventory playerInventory, Inventory merchantInventory)
    {
        WindowManager.OpenTradeWindow(playerInventory, merchantInventory);
    }

    public void CloseCurrentWindow()
    {
        WindowManager.CloseCurrentWindow();
    }

    public bool IsModalWindowOpen()
    {
        return WindowManager.IsModalWindow;
    }

    public void RefreshTombstoneWindow(bool hasEnoughMoney)
    {
        WindowManager.RefreshTombstoneWindow(hasEnoughMoney);
    }

    public void ShowTradeResult(TradeResult result)
    {
        WindowManager.ShowTradeResult(result); 
    }

    public bool IsInventoryOpen()
    {
        return WindowManager.IsInventoryOpen;
    }
    
    public void ShowNotification(string message)
    {
        notificationPopup.Show(message);
    }
    
    private void HandleMarketClosed()
    {
        MarketClosed?.Invoke();
    }

    public void ShowGameOver(GameResult result)
    {
        CloseCurrentWindow();
        
        Hud.IsEnabled = false;
        
        WindowManager.OpenGameOverWindow(result);

    }
}