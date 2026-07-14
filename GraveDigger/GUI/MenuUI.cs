using System;
using System.Collections.Generic;
using GraveDigger;
using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GUI;

public class MenuUI : IUpdatable, IDrawable
{
    public event Action OnStartClicked;
    public event Action OnSettingsClicked;
    public event Action OnExitClicked;
    
    private Button startButton = new Button(Button.UiButtonMode.Color);
    private Button settingsButton = new Button(Button.UiButtonMode.Texture);
    private Button closeButton = new Button(Button.UiButtonMode.Texture);
 
    private readonly List<Button> buttons = new List<Button>();
    
    private readonly Vector2 screenSize;

    public MenuUI(Vector2 screenSize)
    {
        this.screenSize = screenSize;
    }
    
    public void Start()
    {
        ConfigureButtons();
        SubscribeButtonEvents();
        LayoutButtons(screenSize);

        foreach (Button button in buttons)
        {
            button.Start();
        }
    }

    public void Update(GameTime gameTime)
    {
        foreach (Button button in buttons)
        {
            button.Update(gameTime);       
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Button button in buttons)
        {
            button.Draw(spriteBatch);     
        }
    }
    
    private void ConfigureButtons()
    {
        ConfigureStartButton();
        ConfigureSettingsButton();
        ConfigureCloseButton();
    }

    private void SubscribeButtonEvents()
    {
        startButton.OnClick += () => OnStartClicked?.Invoke();
        settingsButton.OnClick += () => OnSettingsClicked?.Invoke();
        closeButton.OnClick += () => OnExitClicked?.Invoke();
    }

    private void LayoutButtons(Vector2 screenSize)
    {
        int ButtonY = 900;
        int ButtonPadding = 50;
        
        int width = startButton.Bounds.Width + 
                    settingsButton.Bounds.Width + 
                    closeButton.Bounds.Width + 
                    ButtonPadding * 2;
        
        int x = (int) (screenSize.X * 0.5f - width * 0.5f);
        startButton.SetPosition(x, ButtonY);
        
        x += startButton.Bounds.Width + ButtonPadding;
        settingsButton.SetPosition(x, ButtonY);
        
        x += settingsButton.Bounds.Width + ButtonPadding;
        closeButton.SetPosition(x, ButtonY);
    }
    
    private void ConfigureStartButton()
    {
        startButton.SetSize(400, 100);
        
        startButton.SetColors(new Color(176, 108, 82),
            new Color(194, 123, 95), 
            new Color(148, 87, 63));
        
        startButton.SetFont(GUIResources.DefaultFont);
        startButton.SetText("Start Game");
        startButton.SetTextColor(Color.Black);
        
        buttons.Add(startButton);
    }

    private void ConfigureSettingsButton()
    {
        settingsButton.SetTextures(
            SpriteManager.GetSprite("ButtonNormal").Texture,
            SpriteManager.GetSprite("ButtonHover").Texture,
            SpriteManager.GetSprite("ButtonPressed").Texture);
        
        settingsButton.SetFont(GUIResources.DefaultFont);
        settingsButton.SetText("Settings");
        settingsButton.SetTextColor(Color.Black);
        
        buttons.Add(settingsButton);
    }

    private void ConfigureCloseButton()
    {
        closeButton.SetTextures(
            SpriteManager.GetSprite("CloseButtonNormal").Texture,
            SpriteManager.GetSprite("CloseButtonHover").Texture,
            SpriteManager.GetSprite("CloseButtonPressed").Texture);
        
        buttons.Add(closeButton);
    }
}