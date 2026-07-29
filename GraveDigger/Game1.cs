using System;
using GraveDigger.Characters;
using GraveDigger.Core;
using GraveDigger.Systems;
using GraveDigger.Utils;
using GraveDigger.Visuals;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GraveDigger;

public class Game1 : Game
{
    private const int DayDuration = 20;
    private const int NightDuration = 20;
    private static readonly Vector2 WorldSize = new(4480, 3840);
    
    private readonly GraphicsDeviceManager graphics;
    
    private SpriteBatch spriteBatch;
    
    private GameplayCoordinator gameplayCoordinator;
    private RandomService randomService;
    private GameContext gameContext;

    private Camera camera;
    private Level level;
    private Gui gui;

    private Texture2D cursorTexture;
   
    private GameState currentGameState = GameState.Menu;

    private KeyboardState previousKeyboardState;
    
    // Indicates whether the game has been started.
    // Used to prevent closing the initial menu with the Escape key.
    private bool gameStarted;
    
    
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
        StartObjects();
        SubscribeToEvents();
        
        SetGameState(GameState.Menu);
        
        gui.Hud.UpdateHunger(level.Player.Hunger, 0, level.Player.MaxHunger);
    }

    protected override void Update(GameTime gameTime)
    {
        if (!IsActive)
        {
            // TODO: investigate missed short key presses after restoring focus.
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
            
            case GameState.GameOver:
                UpdateGameOver(gameTime);
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
        
        switch (currentGameState)
        {
            case GameState.Menu:
                DrawGUI();
                break;

            case GameState.Playing:
            case GameState.GameOver:
                DrawGameplay();
                DrawGUI();
                break;
        }

        base.Draw(gameTime);
    }

    private void DrawGameplay()
    {
        spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, 
            samplerState: SamplerState.PointClamp, 
            transformMatrix: camera.TransformMatrix);
        level.Draw(spriteBatch);
        spriteBatch.End();
            
        spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
        level.DrawOverlay(spriteBatch);
        spriteBatch.End();
            
        spriteBatch.Begin(
            blendState: BlendState.Additive,
            samplerState: SamplerState.LinearClamp,
            transformMatrix: camera.TransformMatrix
        );
        level.DrawLights(spriteBatch);
        spriteBatch.End();    
    }

    private void DrawGUI()
    {
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
    
    private void RestartGame()
    {
        Console.WriteLine("Restarting game... Maybe...");
    }

    private void EndGame(GameResult gameResult)
    {
        gui.ShowGameOver(gameResult);
        SetGameState(GameState.GameOver);
    }
    
    private void OpenSettings()
    {
        // TODO: implement settings or remove button
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
        
        TimeSystem timeSystem = new TimeSystem(DayDuration, NightDuration);
        DayNightOverlay dayNightOverlay = new DayNightOverlay(timeSystem, gameContext.ScreenSize);
        
        level = new Level(gameContext, dayNightOverlay);
        level.LoadContent();
        
        gameplayCoordinator = new GameplayCoordinator(gui, level, timeSystem, randomService);
    }

    private void SubscribeToEvents()
    {
        gui.MenuUi.OnStartClicked += StartGame; 
        gui.MenuUi.OnSettingsClicked += OpenSettings; 
        gui.MenuUi.OnExitClicked += CloseGame;
        
        gui.WindowManager.GameOverWindow.RestartButtonPressed += RestartGame;
        gui.WindowManager.GameOverWindow.ExitButtonPressed += CloseGame;
        
        gameplayCoordinator.GameEnded += EndGame;
            
        level.InteractionSystem.OnHoveredInteractionChanged += gui.InteractionTooltip.SetInteraction;
    }

    private void StartObjects()
    {
        gui.Start();
        level.Start();
        
        gameplayCoordinator.Start();
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
        gameplayCoordinator.Update(gameTime);
    }
    
    private void UpdateGameOver(GameTime gameTime)
    {
        // TODO: see if need anything
        // GameOverWindow is updated through gui.Update().
    }
    
    private void HandleWindowInput(KeyboardState keyboardState)
    {
        if (WasKeyJustPressed(keyboardState, Keys.I))
        {
            gameplayCoordinator.ToggleInventory();
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