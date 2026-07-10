using System.Collections.Generic;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.Interactions;

public class InteractionSystem : IUpdatable
{
    private Interaction hoveredInteraction;
    private MouseState previousMouseState;
    private bool wasPressedInside = false;
    
    private List<Interaction> interactions = new List<Interaction>();
    
    public void RegisterInteraction(Interaction interaction)
    {
        interactions.Add(interaction);
    }

    public void UnregisterInteraction(Interaction interaction)
    {
        if (interactions.Contains(interaction))
        {
            interactions.Remove(interaction);
        }
    }
    
    public void Start()
    {
    }

    public void Update(GameTime gameTime)
    {
        MouseState currentMouseState = Mouse.GetState();
        Vector2 mousePosition = currentMouseState.Position.ToVector2();
        
        Interaction newHoveredInteraction = null;
        bool isHovered = false;
        
        foreach (Interaction interaction in interactions)
        {
            if (interaction.GetArea().Contains(mousePosition))
            {
                newHoveredInteraction = interaction;
                isHovered = true;
                break;
            }
        }
        
        if (newHoveredInteraction != hoveredInteraction)
        {
            hoveredInteraction?.OnHoverExit();
            hoveredInteraction = newHoveredInteraction;
            hoveredInteraction?.OnHoverEnter();
        }
        
        bool mouseJustPressed = currentMouseState.LeftButton == ButtonState.Pressed &&
                                previousMouseState.LeftButton == ButtonState.Released;

        bool mouseJustReleased = currentMouseState.LeftButton == ButtonState.Released &&
                                 previousMouseState.LeftButton == ButtonState.Pressed;
        
        if (isHovered && mouseJustPressed)
            wasPressedInside = true;
        
        if (mouseJustReleased)
        {
            if (wasPressedInside && isHovered)
            {
                hoveredInteraction?.Interact();
            }

            wasPressedInside = false;
        }
        
        previousMouseState = currentMouseState;
    }
}