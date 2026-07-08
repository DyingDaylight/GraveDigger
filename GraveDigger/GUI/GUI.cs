using System;
using System.Collections.Generic;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GUI;

public class GUI: IUpdatable, IDrawable
{
    public event Action OnStartClicked;
    public event Action OnSettingsClicked;
    public event Action OnExitClicked;
    
    private Button startButton = new Button(Button.UiButtonMode.Color);
    private Button settingsButton = new Button(Button.UiButtonMode.Texture);
    private Button closeButton = new Button(Button.UiButtonMode.Texture);

    private SpriteFont font;
    
    private bool started = false;
    
    private readonly List<Button> buttons = new List<Button>();
    private readonly Vector2 screenSize;


    public GUI(Vector2 screenSize)
    {
        this.screenSize = screenSize;
    }
    
    public void LoadContent(ContentManager content)
    {
        GUIResources.LoadContent(content);
        
        font = content.Load<SpriteFont>("Fonts/File");
        
        settingsButton.SetTextures(
            content.Load<Texture2D>($"Images/GUI/ButtonNormal"), 
            content.Load<Texture2D>($"Images/GUI/ButtonHover"),
            content.Load<Texture2D>($"Images/GUI/ButtonPressed"));
        
        closeButton.SetTextures(
            content.Load<Texture2D>($"Images/GUI/CloseButtonNormal"),
            content.Load<Texture2D>($"Images/GUI/CloseButtonHover"),
            content.Load<Texture2D>($"Images/GUI/CloseButtonPressed"));
    }
    
    public void Start()
    {
        // Prevent configuring and subscribing buttons more than once.
        if (started)
            throw new InvalidOperationException("GUI has already been started.");

        started = true;
        
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
        
        startButton.SetFont(font);
        startButton.SetText("Start Game");
        startButton.SetTextColor(Color.Black);
        
        buttons.Add(startButton);
    }

    private void ConfigureSettingsButton()
    {
        settingsButton.SetFont(font);
        settingsButton.SetText("Settings");
        settingsButton.SetTextColor(Color.Black);
        
        buttons.Add(settingsButton);
    }

    private void ConfigureCloseButton()
    {
        buttons.Add(closeButton);
    }
}