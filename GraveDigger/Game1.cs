using System;
using GraveDigger.Characters;
using GraveDigger.Core;
using GraveDigger.Systems;
using GraveDigger.Utils;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GraveDigger;

public class Game1 : Game
{
    private const int DayDuration = 40;
    private const int NightDuration = 40;
    private static readonly Vector2 WorldSize = new(4520, 3960);
    
    private const float TransitionDuration = 0.15f;
    private readonly Color dawnColor = new(130, 92, 98, 28);
    private readonly Color dayColor = Color.Transparent;
    private readonly Color sunsetColor = new(110, 85, 95, 55);
    private readonly Color duskColor = new(50, 45, 70, 90);
    private readonly Color nightColor = new(20, 30, 70, 130);
    
    private GameContext gameContext;
    private GameplayCoordinator gameplayCoordinator;
    private ReputationSystem reputationSystem;
    private RandomService randomService;
    private TimeSystem timeSystem;
    
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    
    private Camera camera;
    private Level level;
    
    private Gui gui;

    private Texture2D cursorTexture;
    private Texture2D dayNightOverlay;
    private GameState currentGameState = GameState.Menu;

    private KeyboardState previousKeyboardState;
    
    // Indicates whether the game has been started.
    // Used to prevent closing the initial menu with the Escape key.
    private bool gameStarted = false;
    
    
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false; // to hide basic cursor
        SpriteManager.Initialize(Content);
    }

    protected override void Initialize()
    {
        //_graphics.IsFullScreen = true;
        graphics.PreferredBackBufferWidth = 1920;
        graphics.PreferredBackBufferHeight = 1080;
        graphics.ApplyChanges();

        Vector2 screenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        
        SortingUtility.Initialize(WorldSize.Y);
        AudioManager.Instance.Initialize(Content);
        
        randomService = new RandomService(1234);
        camera = new Camera(GraphicsDevice.Viewport, WorldSize);
        gameContext = new GameContext(camera, screenSize, WorldSize, randomService);
        
        base.Initialize();
    }
    
    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        cursorTexture = Content.Load<Texture2D>("Images/GUI/cursor"); // custom cursor
        
        LoadCoreSprites();
        CreateGameObjects();
        SubscribeToEvents();
        
        dayNightOverlay = SpriteManager.GetSprite("pixel").Texture;
        
        SetGameState(GameState.Menu);
        gui.Hud.UpdateReputation(reputationSystem.Value,
            ReputationSystem.MinValue, ReputationSystem.MaxValue);
        gui.Hud.UpdateHunger(level.Player.Hunger, 0, level.Player.MaxHunger);
    }

    protected override void Update(GameTime gameTime)
    {
        if (!IsActive)
        {
            //previousKeyboardState = Keyboard.GetState();
            base.Update(gameTime);
            return;
        }
        
        KeyboardState currentKeyboardState = Keyboard.GetState();
        
        AudioManager.Instance.Update(currentGameState);
        
        switch (currentGameState)
        {
            case GameState.Menu:
                UpdateMenu(currentKeyboardState);
                break;
        
            case GameState.Playing:
                UpdateGameplay(gameTime, currentKeyboardState);
                break;
        }
        
        gui.Update(gameTime);
        UpdateCamera(gameTime);
        previousKeyboardState = currentKeyboardState;
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black); 
        
        if (currentGameState == GameState.Playing)
        {
            spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, 
                samplerState: SamplerState.PointClamp, 
                transformMatrix: camera.TransformMatrix);
            level.Draw(spriteBatch);
            spriteBatch.End();
            
            spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
            DrawDayNightOverlay(spriteBatch);
            spriteBatch.End();

            if (timeSystem.CurrentDayTime == DayTime.Night)
            {
                spriteBatch.Begin(
                    blendState: BlendState.Additive,
                    samplerState: SamplerState.LinearClamp,
                    transformMatrix: camera.TransformMatrix
                );
                level.DrawLights(spriteBatch);
                spriteBatch.End();    
            }
        }
        
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend); 
        
        gui.Draw(spriteBatch);
        
        MouseState mouseState = Mouse.GetState();
        spriteBatch.Draw(cursorTexture, new Vector2(mouseState.X, mouseState.Y), Color.White);
        spriteBatch.End();
        
    }
    
    private void DrawDayNightOverlay(SpriteBatch spriteBatch)
    {
        Color overlayColor = GetDayNightOverlayColor();
        
        spriteBatch.Draw(
            dayNightOverlay,
            GraphicsDevice.Viewport.Bounds,
            overlayColor);
    }

    private Color GetDayNightOverlayColor()
    {
        float progress = MathHelper.Clamp(timeSystem.PhaseProgress, 0f, 1f);

        if (timeSystem.CurrentDayTime == DayTime.Day)
        {
            // 0.00–0.15: dawn -> day
            if (progress < TransitionDuration)
            {
                float t = progress / TransitionDuration;
                return Color.Lerp(dawnColor, dayColor, t);
            }

            // 0.15–0.85: day
            if (progress < 1f - TransitionDuration)
                return dayColor;

            // 0.85–1.00: day -> sunset
            float tSunset = (progress - (1f - TransitionDuration)) / TransitionDuration;
            return Color.Lerp(dayColor, sunsetColor, tSunset);
        }

        // 0.00–0.15: sunset -> dusk
        if (progress < TransitionDuration)
        {
            float t = progress / TransitionDuration;
            return Color.Lerp(sunsetColor, duskColor, t);
        }

        // 0.15–0.30: dusk -> night
        if (progress < TransitionDuration * 2f)
        {
            float t = (progress - TransitionDuration) / TransitionDuration;
            return Color.Lerp(duskColor, nightColor, t);
        }

        // 0.30–0.85: night
        if (progress < 1f - TransitionDuration)
            return nightColor;

        // 0.85–1.00: night -> dawn
        float tDawn = (progress - (1f - TransitionDuration)) / TransitionDuration;

        return Color.Lerp(nightColor, dawnColor, tDawn);
    }

    private void StartGame()
    {
        gameStarted = true;
        SetGameState(GameState.Playing);
    }

    private void OpenSettings()
    {
        Console.WriteLine("Settings");
    }
    
    private void CloseGame()
    {
        Exit();
    }
    
    private void LoadCoreSprites()
    {
        SpriteManager.AddSprite("pixel", "Images/pixel");
        SpriteManager.AddSprite("light", "Images/Effects/Light");
    }

    private void CreateGameObjects()
    {
        gui = new Gui(gameContext);
        gui.LoadContent(Content);
        gui.Start();
        
        timeSystem = new TimeSystem(DayDuration, NightDuration);
        reputationSystem = new ReputationSystem();
        gameplayCoordinator = new GameplayCoordinator(timeSystem, gui, reputationSystem, randomService);
        
        level = new Level(gameContext, gameplayCoordinator);
        level.LoadTextures();
        level.Start();
        timeSystem.Start();
    }

    private void SubscribeToEvents()
    {
        gui.MenuUi.OnStartClicked += StartGame; 
        gui.MenuUi.OnSettingsClicked += OpenSettings; 
        gui.MenuUi.OnExitClicked += CloseGame;

        gui.WindowManager.TombstoneInfoWindow.OnDigButton += (tombstone) => gameplayCoordinator.DigGrave(tombstone.ParentSite);
        gui.WindowManager.TombstoneInfoWindow.OnRepairButton += (tombstone) => gameplayCoordinator.RepairGrave(tombstone.ParentSite);
        
        gui.WindowManager.InventoryWindow.UseRequested += gameplayCoordinator.UseItem;
        gui.WindowManager.InventoryWindow.DiscardRequested += gameplayCoordinator.DiscardItem;

        gui.WindowManager.TradeWindow.SellRequested += gameplayCoordinator.SellItem;
        gui.WindowManager.TradeWindow.BuyRequested += gameplayCoordinator.BuyItem;
        gui.WindowManager.TradeWindow.UseRequested += gameplayCoordinator.UseItem;
        gui.WindowManager.TradeWindow.DiscardRequested += gameplayCoordinator.DiscardItem;

        gui.Hud.InventoryRequested += gameplayCoordinator.ShowInventory;
        
        level.Player.HungerChanged += gui.Hud.UpdateHunger;
        reputationSystem.ReputationChanged += gui.Hud.UpdateReputation;
            
        level.InteractionSystem.OnHoveredInteractionChanged += gui.InteractionTooltip.SetInteraction;

        gameplayCoordinator.OnNotificationRequested += gui.ShowNotification;
        gameplayCoordinator.OnLootSpawn += level.SpawnLoot;
        gameplayCoordinator.OnGraveChanged += level.GraveChanged;
        gameplayCoordinator.OnTradeCompleted += gui.ShowTradeResult;
        gameplayCoordinator.OnMarketClosed += level.MarketClosed;
        gameplayCoordinator.OnUndeadSpawned += level.SpawnUndead;
        gameplayCoordinator.OnNutritionReceived += level.DecreaseHunger;
        gameplayCoordinator.TryApplyBlueprint += level.BuildDecoration;

        timeSystem.DayTimeChanged += level.DayTimeChange;
        timeSystem.DayStarted += level.DayStart;
    }

    private void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        gui.SetGameState(currentGameState);
        
        if (currentGameState == GameState.Menu)
        {
            AudioManager.Instance.SetMusicVolume(0.1f);
            
            if (MediaPlayer.State == MediaState.Stopped)
            {
                AudioManager.Instance.PlayMusic("theme1", loop: false);
            }
        }
        else
        {
            AudioManager.Instance.SetMusicVolume(0f);
        }
    }
    
    private void UpdateCamera(GameTime gameTime)
    {
        camera.SetTarget(level.Player.Transform.Position);
        camera.Update(gameTime);
    }
    
    private void UpdateMenu(KeyboardState keyboardState)
    {
        if (!gameStarted)
            return;
        
        if (WasKeyJustPressed(keyboardState, Keys.Escape))
            SetGameState(GameState.Playing);
    }
    
    private void UpdateGameplay(GameTime gameTime, KeyboardState keyboardState)
    {
        HandleWindowInput(keyboardState);

        if (gui.IsModalWindowOpen())
        {
            level.Player.StopMoving();
            return;
        }
        
        if (WasKeyJustPressed(keyboardState, Keys.Escape))
        {
            SetGameState(GameState.Menu);
            return;
        }

        level.Update(gameTime);
        timeSystem.Update(gameTime);
    }
    
    private void HandleWindowInput(KeyboardState keyboardState)
    {
        if (WasKeyJustPressed(keyboardState, Keys.I))
        {
            if (gui.IsInventoryOpen())
                gui.CloseCurrentWindow();
            else if (!gui.IsModalWindowOpen())
                gameplayCoordinator.ShowInventory();
        }

        // TODO: not a real mechant, testing purposes only!!
        if (WasKeyJustPressed(keyboardState, Keys.T))
        {
            Merchant merchant = new Merchant();
            merchant.Inventory = InventoryGenerator.CreateTestInventory();
            gameplayCoordinator.ShowMarket(merchant);
        }
    }
    
    private bool WasKeyJustPressed(KeyboardState currentKeyboardState, Keys key)
    {
        return currentKeyboardState.IsKeyDown(key) &&
               previousKeyboardState.IsKeyUp(key);
    }
}