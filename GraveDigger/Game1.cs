using System;
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
    private static readonly Vector2 WorldSize = new(4520, 3960);
    
    private GameContext gameContext;
    private GameplayCoordinator gameplayCoordinator;
    private ReputationSystem reputationSystem;
    private RandomService randomService;
    
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    private Camera camera;
    private Player player;
    private Level level;
    
    private Gui gui;
    
    private Texture2D cursorTexture;
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
        
        SortingUtility.Initialize(WorldSize.Y);
        
        Vector2 screenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        
        randomService = new RandomService(1234);
        
        AudioManager.Instance.Initialize(Content);
        
        camera = new Camera(GraphicsDevice.Viewport, WorldSize);
        gameContext = new GameContext(camera, screenSize, WorldSize, randomService);
        
        base.Initialize();
    }
    
    
    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        cursorTexture = Content.Load<Texture2D>("Images/GUI/cursor"); // custom cursor
        spriteBatch = new SpriteBatch(GraphicsDevice);
        
        LoadCoreSprites();
        CreateGameObjects();
        SubscribeToEvents();
        
        SetGameState(GameState.Menu);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        
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

        if (MediaPlayer.State == MediaState.Stopped)
        {
            GraveDigger.Systems.AudioManager.Instance.PlayNextMusic();
        }
        
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
            player.Draw(spriteBatch);
            
            spriteBatch.End();
        }
        
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend); 
        gui.Draw(spriteBatch);
        
        MouseState mouseState = Mouse.GetState();
        spriteBatch.Draw(cursorTexture, new Vector2(mouseState.X, mouseState.Y), Color.White);
        spriteBatch.End();
        
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
        SpriteManager.AddSprite("digger", "Images/Characters/keeper_wasd2", columns: 4, rows: 4);
        SpriteManager.AddSprite("pixel", "Images/pixel");
    }

    private void CreateGameObjects()
    {
        gui = new Gui(gameContext);
        gui.LoadContent(Content);
        gui.Start();
        
        reputationSystem = new ReputationSystem();
        gameplayCoordinator = new GameplayCoordinator(gui, reputationSystem, randomService);
        
        level = new Level(gameContext, gameplayCoordinator);
        level.LoadTextures();
        level.Start();
        
        player = new Player(gameContext);
        player.Start();
    }

private void SubscribeToEvents()
{
    gui.MenuUi.OnStartClicked += StartGame; 
    gui.MenuUi.OnSettingsClicked += OpenSettings; 
    gui.MenuUi.OnExitClicked += CloseGame;

    gui.WindowManager.TombstoneInfoWindow.OnDigButton += gameplayCoordinator.DigGrave;
    gui.WindowManager.TombstoneInfoWindow.OnRepairButton += gameplayCoordinator.RepairGrave;
    
    gui.WindowManager.InventoryWindow.UseRequested += gameplayCoordinator.UseItem;
    gui.WindowManager.InventoryWindow.DiscardRequested += gameplayCoordinator.DiscardItem;

    gui.WindowManager.TradeWindow.SellRequested += gameplayCoordinator.SellItem;
    gui.WindowManager.TradeWindow.BuyRequested += gameplayCoordinator.BuyItem;
    gui.WindowManager.TradeWindow.UseRequested += gameplayCoordinator.UseItem;
    gui.WindowManager.TradeWindow.DiscardRequested += gameplayCoordinator.DiscardItem;

    reputationSystem.ReputationChanged += gui.Hud.UpdateReputation;
        
    level.InteractionSystem.OnHoveredInteractionChanged += gui.InteractionTooltip.SetInteraction;

    gameplayCoordinator.OnLootSpawn += level.SpawnLoot;
}
    
private void SetGameState(GameState gameState)
{
    currentGameState = gameState;
    gui.SetGameState(currentGameState);
    
    if (currentGameState == GameState.Menu)
    {
        GraveDigger.Systems.AudioManager.Instance.SetMusicVolume(0.1f);
        
        if (MediaPlayer.State == MediaState.Stopped)
        {
            GraveDigger.Systems.AudioManager.Instance.PlayMusic("theme1", loop: false);
        }
    }
    else
    {
        GraveDigger.Systems.AudioManager.Instance.SetMusicVolume(0f);
    }
}
    private void UpdateCamera(GameTime gameTime)
    {
        camera.SetTarget(player.Transform.Position);
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
            return;

        level.Update(gameTime);
        player.Update(gameTime);

        if (WasKeyJustPressed(keyboardState, Keys.Escape))
            SetGameState(GameState.Menu);
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

        if (WasKeyJustPressed(keyboardState, Keys.T))
        {
            if (!gui.IsModalWindowOpen())
                gameplayCoordinator.ShowMerchant();
        }
    }
    
    private bool WasKeyJustPressed(KeyboardState currentKeyboardState, Keys key)
    {
        return currentKeyboardState.IsKeyDown(key) &&
               previousKeyboardState.IsKeyUp(key);
    }
    }