using System;
using System.Collections.Generic;
using GraveDigger;
using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using GraveDigger.Core;
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
    
    private Button startButton = new Button(Button.UiButtonMode.Texture);
    private Button settingsButton = new Button(Button.UiButtonMode.Texture);
    private Button closeButton = new Button(Button.UiButtonMode.Texture);
 
    private readonly List<Button> buttons = new List<Button>();
    
    private readonly Vector2 screenSize;
    
    private Sprite background;

    public MenuUI(Vector2 screenSize)
    {
        this.screenSize = screenSize;
    }
    
    public void Start()
    {
        
        background = new Sprite("MainMenuBackground");
        background.Transform.Position = Vector2.Zero;
        background.Pivot = Vector2.Zero;
        background.Start();
        
        ConfigureButtons();
        SubscribeButtonEvents();
        LayoutButtons(screenSize, buttonPadding: 40);
        
        foreach (Button button in buttons)
        {
            button.Start();
        }
    }

    public void Update(GameTime gameTime)
    {
        background.Update(gameTime);
        
        foreach (Button button in buttons)
        {
            button.Update(gameTime);       
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
        background.Draw(spriteBatch);
        
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

    private void LayoutButtons(Vector2 screenSize, int buttonPadding)
    {
        int btnWidth = startButton.Bounds.Width;
        int btnHeight = startButton.Bounds.Height;
        
        int totalGroupHeight = (btnHeight * 3) + (buttonPadding * 2);
        
        int x = (int)(screenSize.X - btnWidth - 150);
        
        int startY = (int)((screenSize.Y - totalGroupHeight) * 0.5f);
        
        startButton.SetPosition(x, startY);
        
        int settingsY = startY + btnHeight + buttonPadding;
        settingsButton.SetPosition(x, settingsY);
        
        int exitY = settingsY + btnHeight + buttonPadding;
        closeButton.SetPosition(x, exitY);
    }
    
    //TODO: Button effects
    private void ConfigureStartButton()
    {
        Texture2D mainmenuButtonTex = SpriteManager.GetSprite("ButtonMainMenu").Texture;
        
        startButton.SetTextures(mainmenuButtonTex, mainmenuButtonTex, mainmenuButtonTex);
        startButton.SetFont(GUIResources.LargeFont);
        startButton.SetText("Play");
        startButton.SetTextColor(Color.Black);
        
        buttons.Add(startButton);
    }

    private void ConfigureSettingsButton()
    {
        Texture2D mainmenuButtonTex = SpriteManager.GetSprite("ButtonMainMenu").Texture;
        settingsButton.SetTextures(mainmenuButtonTex, mainmenuButtonTex, mainmenuButtonTex);
        settingsButton.SetFont(GUIResources.LargeFont);
        settingsButton.SetText("Settings");
        settingsButton.SetTextColor(Color.Black);
        
        buttons.Add(settingsButton);
    }

    private void ConfigureCloseButton()
    {
        Texture2D mainmenuButtonTex = SpriteManager.GetSprite("ButtonMainMenu").Texture;
        closeButton.SetTextures(mainmenuButtonTex, mainmenuButtonTex, mainmenuButtonTex);
        closeButton.SetFont(GUIResources.LargeFont);
        closeButton.SetText("Exit");
        closeButton.SetTextColor(Color.Black);
        
        buttons.Add(closeButton);
    }
}