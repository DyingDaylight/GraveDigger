using System;
using System.Collections.Generic;
using System.Linq;
using GraveDigger.GUI.Layouts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.GUI.Elements;

public class ContextMenu : UIContainer
{
    private readonly List<Button> buttons = new();
    private readonly VerticalLayout layout;
    
    private MouseState previousMouseState;
    
    public ContextMenu()
    {
        layout = new VerticalLayout(Bounds);
        Visible = false;
    }

    public override void Update(GameTime gameTime)
    {
        if (!Visible)
            return;

        base.Update(gameTime);

        MouseState currentMouseState = Mouse.GetState();

        bool leftClicked =
            currentMouseState.LeftButton == ButtonState.Pressed &&
            previousMouseState.LeftButton == ButtonState.Released;

        if (leftClicked)
        {
            Point mousePosition = currentMouseState.Position;

            if (!Bounds.Contains(mousePosition))
                Hide();
        }

        previousMouseState = currentMouseState;
    }

    public void Show(Vector2 position, List<ContextMenuAction> actions)
    {
        ClearOptions();

        const int padding = 1;
        const int buttonHeight = 60;
        const int buttonWidth = 250;
        int buttonsCount = actions.Count();

        int menuHeight = buttonsCount * buttonHeight
                         + Math.Max(0, buttonsCount - 1) * padding;
        
        SetSize(buttonWidth, menuHeight);
        SetPosition((int)position.X, (int)position.Y);
        
        foreach (var action in actions)
        {
            Button button = CreateElement<Button>();
            button.SetText(action.Name);
            button.SetSize(buttonWidth, buttonHeight);

            button.OnClick += () => ExecuteAction(action);
            
            buttons.Add(button);
            layout.AddElement(button);
        }
        
        layout.SetBounds(Bounds);
        layout.UpdateLayout();
        Visible = true;
    }

    public void Hide()
    {
        Visible = false;
    }

    private void ClearOptions()
    {
        foreach (var button in buttons)
            RemoveElement(button);
        buttons.Clear();
        layout.RemoveAll();
    }
    
    private void ExecuteAction(ContextMenuAction action)
    {
        action.Execute();
        Hide();
    }
}