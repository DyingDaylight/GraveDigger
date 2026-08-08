using System;
using GraveDigger.GUI.Layouts;
using GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Elements;

public class Button : ClickableUIElement
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
    
    public event Action? OnClick;
    
    private Color normalColor = Color.White;
    private Color hoverColor = Color.LightGray;
    private Color pressedColor = Color.DarkGray;
    private Color disabledColor = Color.Gray;
    
    private Texture2D? normalTexture;
    private Texture2D? hoverTexture;
    private Texture2D? pressedTexture;
    private Texture2D? disabledTexture;

    private UiButtonState currentState;
    private readonly UiButtonMode buttonMode;
    private readonly Label label = new();
    private readonly Image icon = new();
    private readonly HorizontalLayout horizontalLayout;
    
    private bool isDisabled;
    
    public Button() : this(UiButtonMode.Color)
    {
        SetSize(300, 80);
        
        SetColors(
            GUIResources.ButtonNormalColor, 
            GUIResources.ButtonHoverColor,
            GUIResources.ButtonPressedColor,
            GUIResources.ButtonDisabledColor);
    }
    
    public Button(UiButtonMode mode)
    {
        buttonMode = mode;
        
        horizontalLayout = new HorizontalLayout(Bounds);
        horizontalLayout.Alignment = HorizontalLayout.VerticalAlignment.MiddleCenter;
        
        horizontalLayout.AddElement(icon);
        horizontalLayout.AddElement(label);
        
        icon.SetSize(50, 50);
        icon.Visible = false;
        
        SetFont(GUIResources.DefaultFont);
        SetTextColor(Color.Black);
        UpdateContentLayout();
        
        LeftClicked += HandleLeftClick;
    }

    public bool Enabled => !isDisabled;

    public override void Start()
    {
        base.Start();
        label.Start();
        UpdateContentLayout();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        label.Update(gameTime);

        if (isDisabled)
        {
            currentState = UiButtonState.Disabled;
        }
        else
        {
            UpdateInteraction();

            currentState = IsLeftPressed
                ? UiButtonState.Pressed
                : IsHovered
                    ? UiButtonState.Hover
                    : UiButtonState.Normal;
        }

        ApplyState();
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        Rectangle renderBounds = Bounds;

        if (currentState == UiButtonState.Pressed)
        {
            int shrink = 6; 
            renderBounds = new Rectangle(
                Bounds.X + shrink / 2,
                Bounds.Y + shrink / 2,
                Math.Max(1, Bounds.Width - shrink),
                Math.Max(1, Bounds.Height - shrink)
            );
        }

        if (Texture != null)
        {
            spriteBatch.Draw(Texture, renderBounds, Color);
        }

        label.Draw(spriteBatch);
        if (icon.Visible)
            icon.Draw(spriteBatch);
    }

    public void SetText(string text)
    {
        label.Text = text;
        UpdateContentLayout();
    }

    public void SetFont(SpriteFont font)
    {
        label.Font = font;
        UpdateContentLayout();
    }

    public void SetTextColor(Color color)
    {
        label.Color = color;
    }

    public void SetIcon(Texture2D? texture)
    {
        icon.Visible = texture != null;

        if (texture != null)
            icon.SetImage(texture);

        UpdateContentLayout();
    }

    public void SetTextures(Texture2D normal, 
        Texture2D? hover = null, 
        Texture2D? pressed = null, 
        Texture2D? disabled = null)
    {
        if (buttonMode != UiButtonMode.Texture)
            return;
        
        normalTexture = normal;
        hoverTexture = hover;
        pressedTexture = pressed;
        disabledTexture = disabled;
       
        Texture = normalTexture;
        Color = Color.White;
        
        SetSize(Texture.Width, Texture.Height);
    }

    public void SetColors(Color normal, Color hover, Color pressed, Color? disabled = null)
    {
        if (buttonMode != UiButtonMode.Color)
            return;
        
        normalColor = normal;
        hoverColor = hover;
        pressedColor = pressed;
        disabledColor = disabled ?? Color.Gray;
        
        Texture = GUIResources.ButtonDefaultTexture;
        Color = normalColor;
    }
    
    public void SetDisabled(bool disabled)
    {
        isDisabled = disabled;
        currentState = disabled? UiButtonState.Disabled : UiButtonState.Normal;
    }
    
    private void ApplyState()
    {
        if (buttonMode == UiButtonMode.Texture)
            ApplyTextureState();
        else
            ApplyColorState();
    }

    private void ApplyTextureState()
    {
        Texture2D newTexture = currentState switch
            
        {
            UiButtonState.Normal => normalTexture,
            UiButtonState.Hover => hoverTexture ?? normalTexture,
            UiButtonState.Pressed => pressedTexture ?? normalTexture,
            UiButtonState.Disabled => disabledTexture ?? normalTexture,
            _ => normalTexture
        };
        
        Texture = newTexture;
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

    private void HandleLeftClick(Point position)
    {
        OnClick?.Invoke();
    }
    
    public void LockSize(int width, int height)
    {
        base.SetSize(width, height);
    }
    
    private void UpdateContentLayout()
    {
        int contentHeight = Math.Max(
            icon.Visible ? icon.Bounds.Height : 0,
            label.Bounds.Height
        );

        var contentBounds = new Rectangle(
            Bounds.X,
            (int)(Bounds.Y + (Bounds.Height - contentHeight) * 0.5f),
            Bounds.Width,
            contentHeight
        );

        horizontalLayout.SetBounds(contentBounds);
        horizontalLayout.UpdateLayout();
    }

    protected override void RefreshLayout()
    {
        base.RefreshLayout();
        UpdateContentLayout();
    }
}