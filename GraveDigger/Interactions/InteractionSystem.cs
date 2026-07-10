using System;
using System.Collections.Generic;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.Interactions;

public class InteractionSystem : IUpdatable
{
    private MouseState previousMouseState;
    
    private Interaction hoveredInteraction;
    private Interaction pressedInteraction;
    
    private List<Interaction> interactions = new List<Interaction>();
    
    private CoordinatesConverter coordinatesConverter;

    public event Action<Interaction> OnHoveredInteractionChanged;

    public InteractionSystem(CoordinatesConverter coordinatesConverter)
    {
        this.coordinatesConverter = coordinatesConverter;
    }
    
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
        Vector2 screenMouse = currentMouseState.Position.ToVector2();
        Vector2 mousePosition = coordinatesConverter.ScreenToWorld(screenMouse);
        
        Interaction newHoveredInteraction = null;
        
        foreach (Interaction interaction in interactions)
        {
            if (interaction.GetArea().Contains(mousePosition))
            {
                newHoveredInteraction = interaction;
                // TODO: think about overlapping objects
                break;
            }
        }
        
        if (newHoveredInteraction != hoveredInteraction)
        {
            hoveredInteraction?.OnHoverExit();
            hoveredInteraction = newHoveredInteraction;
            hoveredInteraction?.OnHoverEnter();
            OnHoveredInteractionChanged?.Invoke(hoveredInteraction);
        }
        
        bool mouseJustPressed = currentMouseState.LeftButton == ButtonState.Pressed &&
                                previousMouseState.LeftButton == ButtonState.Released;

        bool mouseJustReleased = currentMouseState.LeftButton == ButtonState.Released &&
                                 previousMouseState.LeftButton == ButtonState.Pressed;
        
        if (newHoveredInteraction != null && mouseJustPressed)
            pressedInteraction = newHoveredInteraction;
        
        if (mouseJustReleased)
        {
            if (newHoveredInteraction != null && pressedInteraction == newHoveredInteraction)
            {
                hoveredInteraction?.Interact();
            }

            pressedInteraction = null;
        }
        
        previousMouseState = currentMouseState;
    }

    public void ClearState()
    {
        if (hoveredInteraction != null) 
            hoveredInteraction.OnHoverExit();
        hoveredInteraction = null;
        pressedInteraction = null;
        OnHoveredInteractionChanged(null);
    }
}