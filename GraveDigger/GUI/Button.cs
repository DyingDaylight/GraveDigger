using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GUI;

public class Button : UIElement
{
    public enum UiButtonState
    {
        Normal,
        Hover,
        Pressed
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
    
    private Texture2D normalTexture;
    private Texture2D hoverTexture;
    private Texture2D pressedTexture;

    private UiButtonState currentState;
    
    private readonly UiButtonMode buttonMode;
    
    private Label label = new Label();
    
    private ButtonState previousMouseButtonState;
    private bool wasPressedInside;

    public Button() : this(UiButtonMode.Color)
    {
    }
    
    public Button(UiButtonMode mode)
    {
        buttonMode = mode;
    }
    
    public override void Start()
    {
        label.Start();
        base.Start();
    }
    
    public override void Update(GameTime gameTime)
    {
        UpdateLabel(gameTime);

        MouseState mouse = Mouse.GetState();
        bool isHover = IsMouseOver(mouse);

        HandleClick(mouse, isHover);

        currentState = GetStateFromMouse(mouse, isHover);

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

    public void SetTextures(Texture2D normal, Texture2D hover, Texture2D pressed)
    {
        if (buttonMode != UiButtonMode.Texture)
            return;

        if (normal == null)
            throw new ArgumentNullException("Button normal state is not set");
        
        normalTexture = normal;
        hoverTexture = hover;
        pressedTexture = pressed;
       
        Texture = normalTexture;
        Color = Color.White;
        SetSize(Texture.Width, Texture.Height);
    }

    public void SetColors(Color normal, Color hover, Color pressed)
    {
        if (buttonMode != UiButtonMode.Color)
            return;
        
        normalColor = normal;
        hoverColor = hover;
        pressedColor = pressed;
        
        Texture = GUIResources.ButtonDefaultTexture;
        Color = normalColor;
    }
    
    private void UpdateLabel(GameTime gameTime)
    {
        label.CenterIn(Bounds);
        label.Update(gameTime);
    }
    
    private bool IsMouseOver(MouseState mouse)
    {
        var mousePos = new Point(mouse.X, mouse.Y);
        return Bounds.Contains(mousePos);
    }
    
    // A click is registered only if the mouse was pressed and released inside the button.
    private void HandleClick(MouseState mouse, bool isHover)
    {
        bool mouseJustPressed = mouse.LeftButton == ButtonState.Pressed &&
                                previousMouseButtonState == ButtonState.Released;

        bool mouseJustReleased = mouse.LeftButton == ButtonState.Released &&
                                 previousMouseButtonState == ButtonState.Pressed;

        if (isHover && mouseJustPressed)
        {
            wasPressedInside = true;
        }

        if (mouseJustReleased)
        {
            if (wasPressedInside && isHover)
            {
                OnClick?.Invoke();
            }

            wasPressedInside = false;
        }

        previousMouseButtonState = mouse.LeftButton;
    }
    
    private UiButtonState GetStateFromMouse(MouseState mouse, bool isHover)
    {
        if (isHover && mouse.LeftButton == ButtonState.Pressed)
            return UiButtonState.Pressed;

        if (isHover)
            return UiButtonState.Hover;

        return UiButtonState.Normal;
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
            _ => normalColor
        };
    }
}