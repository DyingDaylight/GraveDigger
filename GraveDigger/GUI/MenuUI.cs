using System;
using System.Collections.Generic;
using GraveDigger;
using GraveDigger.Core;
using GraveDigger.GUI.Elements;
using GraveDigger.Core;
using GraveDigger.GUI.Components;
using GraveDigger.GUI.Layouts;
using GraveDigger.Systems;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;


namespace GUI;

public class MenuUI : IUpdatable, IDrawable
{
    private enum MenuState
    {
        Main,
        Settings
    }

    private MenuState currentState = MenuState.Main;

    public event Action OnStartClicked;
    public event Action OnSettingsClicked;
    public event Action OnExitClicked;
    public event Action<bool> OnFullScreenToggled;

    private readonly List<UIElement> mainElements = new();
    private readonly List<UIElement> settingsElements = new();

    private VerticalLayout mainLayout;
    private VerticalLayout settingsLayout;

    private Button startButton = new (Button.UiButtonMode.Texture);
    private Button settingsButton = new (Button.UiButtonMode.Texture);
    private Button closeButton = new (Button.UiButtonMode.Texture);

    private VolumeSlider generalSoundSlider;
    private VolumeSlider musicSlider;
    private VolumeSlider effectsSlider;
    
    private Button fullscreenButton = new(Button.UiButtonMode.Texture);
    private Button backButton = new(Button.UiButtonMode.Texture);

    private bool isFullScreen;
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

        int menuWidth = 400;
        int menuHeight = (int)(screenSize.Y * 0.8f);
        int menuX = (int)(screenSize.X - menuWidth - 100);
        int menuY = (int)((screenSize.Y - menuHeight) * 0.5f);
        Rectangle menuBounds = new Rectangle(menuX, menuY, menuWidth, menuHeight);

        mainLayout = new VerticalLayout(menuBounds)
        {
            VerticalPadding = 40,
            Alignment = VerticalLayout.HorizontalAlignment.Center
        };

        settingsLayout = new VerticalLayout(menuBounds)
        {
            VerticalPadding = 25,
            Alignment = VerticalLayout.HorizontalAlignment.Center
        };

        ConfigureMainPanel();
        ConfigureSettingsPanel();
        SubscribeEvents();

        mainLayout.UpdateLayout();
        settingsLayout.UpdateLayout();

        foreach (var element in mainElements)
            element.Start();

        foreach (var element in settingsElements)
            element.Start();

        SetState(MenuState.Main);
    }

    public void Update(GameTime gameTime)
    {
        background.Update(gameTime);

        if (currentState == MenuState.Main)
        {
            foreach (var element in mainElements)
                element.Update(gameTime);
        }
        else if (currentState == MenuState.Settings)
        {
            foreach (var element in settingsElements)
                element.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        background.Draw(spriteBatch);

        if (currentState == MenuState.Main)
        {
            foreach (var element in mainElements)
                element.Draw(spriteBatch);
        }
        else if (currentState == MenuState.Settings)
        {
            foreach (var element in settingsElements)
                element.Draw(spriteBatch);
        }
    }

    private void SetState(MenuState state)
    {
        currentState = state;
    }

    private void ConfigureMainPanel()
    {
        Texture2D mainmenuButtonTex = SpriteManager.GetSprite("ButtonMainMenu").Texture;
        Texture2D hoverTex = SpriteManager.GetSprite("ButtonHover").Texture;
        Texture2D pressedTex = SpriteManager.GetSprite("ButtonPressed").Texture;

        SetupButton(startButton, "Play", mainmenuButtonTex, hoverTex, pressedTex);
        SetupButton(settingsButton, "Settings", mainmenuButtonTex, hoverTex, pressedTex);
        SetupButton(closeButton, "Exit", mainmenuButtonTex, hoverTex, pressedTex);

        AddMainElement(startButton);
        AddMainElement(settingsButton);
        AddMainElement(closeButton);
    }

    private void ConfigureSettingsPanel()
    {
        Texture2D lineTex = SpriteManager.GetSprite("ReputationLineIcon").Texture;
        Texture2D sliderTex = SpriteManager.GetSprite("ReputationSliderIcon").Texture;
        SpriteFont font = GUIResources.DefaultFont;

        generalSoundSlider = new VolumeSlider("General Sound", AudioManager.Instance.SoundVolume, lineTex, sliderTex, font);
        generalSoundSlider.OnValueChanged += (val) => AudioManager.Instance.SoundVolume = val;
        AddSettingsElement(generalSoundSlider);

        musicSlider = new VolumeSlider("Music", AudioManager.Instance.MusicVolume, lineTex, sliderTex, font);
        musicSlider.OnValueChanged += (val) => AudioManager.Instance.MusicVolume = val;
        AddSettingsElement(musicSlider);
        
        effectsSlider = new VolumeSlider("Effects", AudioManager.Instance.SfxVolume, lineTex, sliderTex, font);
        effectsSlider.OnValueChanged += (val) => AudioManager.Instance.SfxVolume = val;
        AddSettingsElement(effectsSlider);

        Texture2D fsOffTex = SpriteManager.GetSprite("FullScreenIcon").Texture;
        Texture2D fsHoverTex = SpriteManager.GetSprite("FullScreenIconHover").Texture;

        fullscreenButton.SetTextures(fsOffTex, fsHoverTex, fsHoverTex);
        fullscreenButton.LockSize(100, 100);
        fullscreenButton.SetFont(font);
        fullscreenButton.SetText("Full Screen Mode");
        fullscreenButton.SetTextColor(Color.White);
        AddSettingsElement(fullscreenButton);

        Texture2D mainmenuButtonTex = SpriteManager.GetSprite("ButtonMainMenu").Texture;
        Texture2D hoverTex = SpriteManager.GetSprite("ButtonHover").Texture;
        Texture2D pressedTex = SpriteManager.GetSprite("ButtonPressed").Texture;

        SetupButton(backButton, "Back", mainmenuButtonTex, hoverTex, pressedTex);
        AddSettingsElement(backButton);
    }

    private void SetupButton(Button button, string text, Texture2D normal, Texture2D hover, Texture2D pressed)
    {
        button.SetTextures(normal, hover, pressed);
        button.SetFont(GUIResources.LargeFont);
        button.SetText(text);
        button.SetTextColor(Color.Black);
        button.MouseEntered += () => AudioManager.Instance.PlaySFX("scratch");
    }

    private void AddMainElement(UIElement element)
    {
        mainElements.Add(element);
        mainLayout.AddElement(element);
    }

    private void AddSettingsElement(UIElement element)
    {
        settingsElements.Add(element);
        settingsLayout.AddElement(element);
    }

    private void SubscribeEvents()
    {
        startButton.OnClick += () => OnStartClicked?.Invoke();
        settingsButton.OnClick += () => SetState(MenuState.Settings);
        closeButton.OnClick += () => OnExitClicked?.Invoke();
        backButton.OnClick += () => SetState(MenuState.Main);

        fullscreenButton.OnClick += () =>
        {
            isFullScreen = !isFullScreen;
            OnFullScreenToggled?.Invoke(isFullScreen);
        };
    }
}

/*
public class MenuUI : IUpdatable, IDrawable
{
    
    private enum MenuState
    {
        Main,
        Settings
    }
    
    private MenuState currentState = MenuState.Main;
    
    public event Action OnStartClicked;
    public event Action OnSettingsClicked;
    public event Action OnExitClicked;
    
    private Button startButton = new Button(Button.UiButtonMode.Texture);
    private Button settingsButton = new Button(Button.UiButtonMode.Texture);
    private Button closeButton = new Button(Button.UiButtonMode.Texture);
 
    private readonly List<Button> mainButtons = new List<Button>();
    
    private VolumeSlider generalSoundSlider;
    private VolumeSlider musicSlider;
    
    private Button fullscreenOffButton = new Button(Button.UiButtonMode.Texture);
    private Button fullscreenOnButton = new Button(Button.UiButtonMode.Texture);
    
    private Button backButton = new Button(Button.UiButtonMode.Texture);
    
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
        ConfigureSettingsUI();
        SubscribeButtonEvents();
        
        LayoutMainButtons(screenSize, buttonPadding: 40);
        LayoutSettingsUI(screenSize, buttonPadding: 40);
        
        foreach (Button button in mainButtons)
        {
            button.Start();
        }
        
        backButton.Start();
    }

    public void Update(GameTime gameTime)
    {
        background.Update(gameTime);
        
        if (currentState == MenuState.Main)
        {
            foreach (Button button in mainButtons)
            {
                button.Update(gameTime);       
            }
        }
        else if (currentState == MenuState.Settings)
        {
            generalSoundSlider?.Update(gameTime);
            musicSlider?.Update(gameTime);
            
            
            backButton.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        
        background.Draw(spriteBatch);
        
        if (currentState == MenuState.Main)
        {
            foreach (Button button in mainButtons)
            {
                button.Draw(spriteBatch);     
            }
        }
        else if (currentState == MenuState.Settings)
        {
            generalSoundSlider?.Draw(spriteBatch);
            musicSlider?.Draw(spriteBatch);
            backButton.Draw(spriteBatch);
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
        
        settingsButton.OnClick += () => 
        {
            currentState = MenuState.Settings;
            OnSettingsClicked?.Invoke();
        };
        
        closeButton.OnClick += () => OnExitClicked?.Invoke();

        backButton.OnClick += () => 
        {
            currentState = MenuState.Main;
        };
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
    
    private void LayoutMainButtons(Vector2 screenSize, int buttonPadding)
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

    private void LayoutSettingsUI(Vector2 screenSize, int buttonPadding)
    {
        int btnWidth = startButton.Bounds.Width;
        int btnHeight = startButton.Bounds.Height;
        
        int totalGroupHeight = (btnHeight * 3) + (buttonPadding * 2);
        int x = (int)(screenSize.X - btnWidth - 150);
        int startY = (int)((screenSize.Y - totalGroupHeight) * 0.5f);

        generalSoundSlider.SetPosition(x, startY);
        
        int musicY = startY + btnHeight + buttonPadding;
        musicSlider.SetPosition(x, musicY);
        
        int backY = musicY + btnHeight + buttonPadding;
        backButton.SetPosition(x, backY);
    }
    
    //TODO: Button effects
    private void ConfigureStartButton()
    {
        Texture2D mainmenuButtonTex = SpriteManager.GetSprite("ButtonMainMenu").Texture;
        Texture2D hoverTex = SpriteManager.GetSprite("ButtonHover").Texture;
        Texture2D pressedTex = SpriteManager.GetSprite("ButtonPressed").Texture;

        startButton.SetTextures(mainmenuButtonTex, hoverTex, pressedTex);
        startButton.SetFont(GUIResources.LargeFont);
        startButton.SetText("Play");
        startButton.SetTextColor(Color.Black);
        
        startButton.MouseEntered += () => GraveDigger.Systems.AudioManager.Instance.PlaySFX("scratch");
        
        mainButtons.Add(startButton);
    }
    
    private void ConfigureSettingsUI()
    {
        Texture2D lineTex = SpriteManager.GetSprite("ReputationLineIcon").Texture;
        Texture2D sliderTex = SpriteManager.GetSprite("ReputationSliderIcon").Texture;
        SpriteFont font = GUIResources.DefaultFont ?? GUIResources.LargeFont;

        generalSoundSlider = new VolumeSlider("General Sound", AudioManager.Instance.SoundVolume, lineTex, sliderTex, font);
        generalSoundSlider.OnValueChanged += (val) => AudioManager.Instance.SoundVolume = val;

        musicSlider = new VolumeSlider("Music", AudioManager.Instance.MusicVolume, lineTex, sliderTex, font);
        musicSlider.OnValueChanged += (val) => AudioManager.Instance.MusicVolume = val;

        Texture2D mainmenuButtonTex = SpriteManager.GetSprite("ButtonMainMenu").Texture;
        Texture2D hoverTex = SpriteManager.GetSprite("ButtonHover").Texture;
        Texture2D pressedTex = SpriteManager.GetSprite("ButtonPressed").Texture;

        backButton.SetTextures(mainmenuButtonTex, hoverTex, pressedTex);
        backButton.SetFont(GUIResources.LargeFont);
        backButton.SetText("Back");
        backButton.SetTextColor(Color.Black);
        backButton.MouseEntered += () => AudioManager.Instance.PlaySFX("scratch");
    }

    private void ConfigureSettingsButton()
    {
        Texture2D mainmenuButtonTex = SpriteManager.GetSprite("ButtonMainMenu").Texture;
        Texture2D hoverTex = SpriteManager.GetSprite("ButtonHover").Texture;
        Texture2D pressedTex = SpriteManager.GetSprite("ButtonPressed").Texture;
        
        settingsButton.SetTextures(mainmenuButtonTex, hoverTex, pressedTex);
        settingsButton.SetFont(GUIResources.LargeFont);
        settingsButton.SetText("Settings");
        settingsButton.SetTextColor(Color.Black);
        
        settingsButton.MouseEntered += () => GraveDigger.Systems.AudioManager.Instance.PlaySFX("scratch");
        
        mainButtons.Add(settingsButton);
    }

    private void ConfigureCloseButton()
    {
        Texture2D mainmenuButtonTex = SpriteManager.GetSprite("ButtonMainMenu").Texture;
        Texture2D hoverTex = SpriteManager.GetSprite("ButtonHover").Texture;
        Texture2D pressedTex = SpriteManager.GetSprite("ButtonPressed").Texture;

        closeButton.SetTextures(mainmenuButtonTex, hoverTex, pressedTex);
        closeButton.SetFont(GUIResources.LargeFont);
        closeButton.SetText("Exit");
        closeButton.SetTextColor(Color.Black);
        
        closeButton.MouseEntered += () => GraveDigger.Systems.AudioManager.Instance.PlaySFX("scratch");
        
        mainButtons.Add(closeButton);
    }
}

*/