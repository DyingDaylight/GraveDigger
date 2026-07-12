using System;
using GraveDigger.Utils;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger;

public class Game1 : Game
{
    public enum GameState
    {
        Menu,
        Playing
    }
    
    public static Vector2 ScreenSize = new Vector2(1920, 1080);
    public static readonly Vector2 WorldSize = new Vector2(4520, 3960);
    
    private GameState currentGameState = GameState.Menu;
    private GameContext gameContext;
    private GameplayCoordinator gameplayCoordinator;
    private ReputationsSystem reputationsSystem;
    private RandomService randomService;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteManager _spriteManager;

    private Camera camera;
    private Player player;
    private Level level;
    
    private Gui gui;
    
    private KeyboardState previousKeyboardState;
    
    // Indicates whether the game has been started.
    // Used to prevent closing the initial menu with the Escape key.
    private bool gameStarted = false;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _spriteManager = new SpriteManager(Content);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        //_graphics.IsFullScreen = true;
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();

        ScreenSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        camera = new Camera(GraphicsDevice.Viewport);
        randomService = new RandomService(1234);
        gameContext = new GameContext(camera, ScreenSize, randomService);
        reputationsSystem = new ReputationsSystem();
        
        base.Initialize();
    }
    
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        SpriteManager.AddSprite("digger", "Images/Characters/keeper_wasd2", columns: 4, rows: 4);
            //SpriteManager.AddSprite("digger_idle", "Images/Characters/keeper_idle", columns: 4, rows: 1);
        SpriteManager.AddSprite("pixel", "Images/pixel");
        
        gui = new Gui(gameContext);
        gui.LoadContent(Content);
        gui.Start();
        
        gameplayCoordinator = new GameplayCoordinator(gui, reputationsSystem, randomService);
        
        level = new Level(gameContext, gameplayCoordinator);
        level.LoadTextures();
        level.Start();
        
        player = new Player();
        player.Start();
        
        gui.MenuUi.OnStartClicked += StartGame; 
        gui.MenuUi.OnSettingsClicked += OpenSettings; 
        gui.MenuUi.OnExitClicked += CloseGame;

        gui.WindowManager.TombstoneInfoWindow.OnDigButton += gameplayCoordinator.DigGrave;
        gui.WindowManager.TombstoneInfoWindow.OnRepairButton += gameplayCoordinator.RepairGrave;

        reputationsSystem.ReputationChanged += gui.Hud.UpdateReputation;
        
        level.InteractionSystem.OnHoveredInteractionChanged += gui.InteractionTooltip.SetInteraction;

        gameplayCoordinator.OnLootSpawn += level.SpawnLoot;
        
        SetGameState(GameState.Menu);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();
        
        if (currentGameState == GameState.Menu)
        {
            if (gameStarted && currentKeyboardState.IsKeyDown(Keys.Escape) && previousKeyboardState.IsKeyUp(Keys.Escape))
                SetGameState(GameState.Playing);
        } 
        else if (currentGameState == GameState.Playing && !gui.IsModalWindowOpen())
        {
             
            bool inventoryJustPressed =
                currentKeyboardState.IsKeyDown(Keys.I) &&
                previousKeyboardState.IsKeyUp(Keys.I);

            if (inventoryJustPressed)
            {
                if (gui.IsInventoryOpen())
                    gui.CloseCurrentWindow();
                else if (!gui.IsModalWindowOpen())
                    gameplayCoordinator.ShowInventory();
            }

            if (!gui.IsModalWindowOpen())
            {
                level.Update(gameTime);
                player.Update(gameTime);

                if (currentKeyboardState.IsKeyDown(Keys.Escape) &&
                    previousKeyboardState.IsKeyUp(Keys.Escape))
                {
                    SetGameState(GameState.Menu);
                }
            }
        }
        
        gui.Update(gameTime);
        previousKeyboardState = currentKeyboardState;
        
        camera.SetTarget(player.Transform.Position);
        camera.Update(gameTime);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkRed);

        // BackToFront sorting uses sprite layer depth to draw objects in the correct order.
        _spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront,
            samplerState: SamplerState.PointClamp,
            blendState: BlendState.AlphaBlend,
            transformMatrix: camera.TransformMatrix);
        level.Draw(_spriteBatch);
        player.Draw(_spriteBatch);
        
        _spriteBatch.End();

        _spriteBatch.Begin();
        gui.Draw(_spriteBatch);
        _spriteBatch.End();

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
    
    private void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        gui.SetGameState(currentGameState);
    }
}