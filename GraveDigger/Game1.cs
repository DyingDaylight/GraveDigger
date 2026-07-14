using System;
using GraveDigger.Core;
using GraveDigger.Systems;
using GraveDigger.Utils;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger;

public class Game1 : Game
{
    private static readonly Vector2 WorldSize = new(4520, 3960);
    
    private readonly GraphicsDeviceManager graphics;

    private GameplayCoordinator gameplayCoordinator;
    private ReputationSystem reputationSystem;
    private RandomService randomService;
    private GameContext gameContext;
    private SpriteBatch spriteBatch;

    private Camera camera;
    private Player player;
    private Level level;
    private Gui gui;

    private GameState currentGameState = GameState.Menu;
    private KeyboardState previousKeyboardState;
    
    // Indicates whether the game has been started.
    // Used to prevent closing the initial menu with the Escape key.
    private bool gameStarted = false;
    
    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        SpriteManager.Initialize(Content);
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        //_graphics.IsFullScreen = true;
        graphics.PreferredBackBufferWidth = 1920;
        graphics.PreferredBackBufferHeight = 1080;
        graphics.ApplyChanges();

        Vector2 screenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

        SortingUtility.Initialize(WorldSize.Y);
        reputationSystem = new ReputationSystem();
        randomService = new RandomService(1234);
        camera = new Camera(GraphicsDevice.Viewport, WorldSize);
        gameContext = new GameContext(camera, screenSize, WorldSize, randomService);

        base.Initialize();
    }
    
    protected override void LoadContent()
    {
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
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkRed);

        // BackToFront sorting uses sprite layer depth to draw objects in the correct order.
        spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront,
            samplerState: SamplerState.PointClamp,
            blendState: BlendState.AlphaBlend,
            transformMatrix: camera.TransformMatrix);
        level.Draw(spriteBatch);
        player.Draw(spriteBatch);
        
        spriteBatch.End();

        spriteBatch.Begin();
        gui.Draw(spriteBatch);
        spriteBatch.End();

        base.Draw(gameTime);
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

        reputationSystem.ReputationChanged += gui.Hud.UpdateReputation;
        
        level.InteractionSystem.OnHoveredInteractionChanged += gui.InteractionTooltip.SetInteraction;

        gameplayCoordinator.OnLootSpawn += level.SpawnLoot;
    }
    
    private void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        gui.SetGameState(currentGameState);
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