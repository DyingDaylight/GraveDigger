using System;
using System.Collections.Generic;
using GraveDigger;
using GraveDigger.GUI.Elements;
using GraveDigger.GUI.Windows;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GUI;

public class Gui: IUpdatable, IDrawable
{
    public InteractionTooltip interactionTooltip;
    public WindowManager WindowManager { get; }
    public MenuUI MenuUi;
    
    private bool started = false;
    private Game1.GameState gameState;

    private GameContext gameContext;


    public Gui(GameContext gameContext)
    {
        this.gameContext = gameContext;
        
        WindowManager = new WindowManager();
        interactionTooltip = new InteractionTooltip(gameContext.CoordinatesConverter);
        MenuUi = new MenuUI(gameContext.ScreenSize);
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
}