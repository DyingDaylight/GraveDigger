using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.GUI.Elements;

public abstract class ClickableUIElement : UIElement
{
    private ButtonState previousLeftButtonState = ButtonState.Released;
    private ButtonState previousRightButtonState = ButtonState.Released;
    
    private bool leftPressedInside;
    private bool rightPressedInside;
    private bool wasHovered;
    
    protected bool IsHovered { get; private set; }
    protected bool IsLeftPressed { get; private set; }
    protected bool IsRightPressed { get; private set; }
    
    public event Action LeftClicked;
    public event Action RightClicked;

    public event Action MouseEntered;
    public event Action MouseExited;

    public event Action LeftPressed;
    public event Action LeftReleased;

    public event Action RightPressed;
    public event Action RightReleased;
    
    protected void UpdateInteraction()
    {
        MouseState mouse = Mouse.GetState();

        UpdateHover(mouse);
        UpdateLeftButton(mouse);
        UpdateRightButton(mouse);
    }
    
    private void UpdateHover(MouseState mouse)
    {
        IsHovered = IsMouseOver(mouse);

        if (IsHovered && !wasHovered)
            MouseEntered?.Invoke();
        else if (!IsHovered && wasHovered)
            MouseExited?.Invoke();

        wasHovered = IsHovered;
    }
    
    private void UpdateLeftButton(MouseState mouse)
    {
        ButtonState currentState = mouse.LeftButton;

        bool justPressed =
            currentState == ButtonState.Pressed &&
            previousLeftButtonState == ButtonState.Released;

        bool justReleased =
            currentState == ButtonState.Released &&
            previousLeftButtonState == ButtonState.Pressed;

        if (justPressed && IsHovered)
        {
            leftPressedInside = true;
            LeftPressed?.Invoke();
        }

        if (justReleased && leftPressedInside)
        {
            LeftReleased?.Invoke();

            if (IsHovered)
                LeftClicked?.Invoke();

            leftPressedInside = false;
        }

        IsLeftPressed =
            IsHovered &&
            currentState == ButtonState.Pressed &&
            leftPressedInside;

        previousLeftButtonState = currentState;
    }
    
    private void UpdateRightButton(MouseState mouse)
    {
        ButtonState currentState = mouse.RightButton;

        bool justPressed =
            currentState == ButtonState.Pressed &&
            previousRightButtonState == ButtonState.Released;

        bool justReleased =
            currentState == ButtonState.Released &&
            previousRightButtonState == ButtonState.Pressed;

        if (justPressed && IsHovered)
        {
            rightPressedInside = true;
            RightPressed?.Invoke();
        }

        if (justReleased && rightPressedInside)
        {
            RightReleased?.Invoke();

            if (IsHovered)
                RightClicked?.Invoke();

            rightPressedInside = false;
        }

        IsRightPressed =
            IsHovered &&
            currentState == ButtonState.Pressed &&
            rightPressedInside;

        previousRightButtonState = currentState;
    }
    
    private bool IsMouseOver(MouseState mouse)
    {
        var mousePos = new Point(mouse.X, mouse.Y);
        return Bounds.Contains(mousePos);
    }
}