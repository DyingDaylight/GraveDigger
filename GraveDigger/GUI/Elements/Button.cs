using System;
using GUI;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.GUI.Elements;

public class Button : ClickableUIElement, IUISizable
{
    public enum UiButtonState
    {
        Normal,
        Hover,
        Pressed,
        Disabled
    }
    
    public enum UiButtonMode
    {
        Color,
        Texture
    }
    
    public event Action OnClick;
    
    private Color normalColor = Color.White;
    private Color hoverColor = Color.LightGray;
    private Color pressedColor = Color.DarkGray;
    private Color disabledColor = Color.Gray;
    
    private Texture2D normalTexture;
    private Texture2D hoverTexture;
    private Texture2D pressedTexture;
    private Texture2D disabledTexture;

    private UiButtonState currentState;
    
    private readonly UiButtonMode buttonMode;
    
    private Label label = new Label();
    
    private bool isDisabled = false;
    
    public Button() : this(UiButtonMode.Color)
    {
        SetSize(300, 80);
        SetColors(GUIResources.ButtonNormalColor, 
            GUIResources.ButtonHoverColor,
            GUIResources.ButtonPressedColor,
            GUIResources.ButtonDisabledColor);
        SetFont(GUIResources.DefaultFont);
        SetTextColor(Color.Black);
    }
    
    public Button(UiButtonMode mode)
    {
        buttonMode = mode;
        LeftClicked += HandleLeftClick;
    }

    public override void Start()
    {
        label.Start();
        base.Start();
    }

    public override void Update(GameTime gameTime)
    {
        UpdateLabel(gameTime);

        if (!isDisabled)
        {
            UpdateInteraction();

            currentState = IsLeftPressed
                ? UiButtonState.Pressed
                : IsHovered
                    ? UiButtonState.Hover
                    : UiButtonState.Normal;
        }
        else
        {
            currentState = UiButtonState.Disabled;
        }

        ApplyState();
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        label.Draw(spriteBatch);
    }

    public void SetText(string text)
    {
        label.Text = text;
    }

    public void SetFont(SpriteFont font)
    {
        label.Font = font;
    }

    public void SetTextColor(Color color)
    {
        label.Color = color;
    }

    public void SetTextures(Texture2D normal, Texture2D hover, Texture2D pressed, Texture2D disabled = null)
    {
        if (buttonMode != UiButtonMode.Texture)
            return;

        if (normal == null)
            throw new ArgumentNullException("Button normal state is not set");
        
        normalTexture = normal;
        hoverTexture = hover;
        pressedTexture = pressed;
        disabledTexture = disabled;
       
        Texture = normalTexture;
        Color = Color.White;
        SetSize(Texture.Width, Texture.Height);
    }

    public void SetColors(Color normal, Color hover, Color pressed, Color disabled = default)
    {
        if (buttonMode != UiButtonMode.Color)
            return;
        
        normalColor = normal;
        hoverColor = hover;
        pressedColor = pressed;
        if (disabled == default)
            disabledColor = Color.Gray;
        else
            disabledColor = disabled;
        
        Texture = GUIResources.ButtonDefaultTexture;
        Color = normalColor;
    }
    
    private void UpdateLabel(GameTime gameTime)
    {
        label.CenterIn(Bounds);
        label.Update(gameTime);
    }
    
    private void ApplyState()
    {
        if (buttonMode == UiButtonMode.Texture)
        {
            ApplyTextureState();
        }
        else
        {
            ApplyColorState();
        }
    }

    private void ApplyTextureState()
    {
        Texture = currentState switch
        {
            UiButtonState.Normal => normalTexture,
            UiButtonState.Hover => hoverTexture ?? normalTexture,
            UiButtonState.Pressed => pressedTexture ?? normalTexture,
            UiButtonState.Disabled => disabledTexture ?? normalTexture,
            _ => normalTexture
        };
    }
    
    private void ApplyColorState()
    {
        Color = currentState switch
        {
            UiButtonState.Normal => normalColor,
            UiButtonState.Hover => hoverColor,
            UiButtonState.Pressed => pressedColor,
            UiButtonState.Disabled => disabledColor,
            _ => normalColor
        };
    }

    public void SetDisabled(bool disabled)
    {
        isDisabled = disabled;
        if (disabled)
        {
            currentState = UiButtonState.Disabled;
        }
        else
        {
            currentState = UiButtonState.Normal;
        }
    }

    private void HandleLeftClick()
    {
        OnClick?.Invoke();
    }

}