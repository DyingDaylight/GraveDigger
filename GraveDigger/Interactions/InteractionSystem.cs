using System;
using System.Collections.Generic;
using GraveDigger.Utils;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.Interactions;

public class InteractionSystem : IUpdatable
{
    private MouseState previousMouseState;
    
    private Interaction hoveredInteraction;
    private Interaction pressedInteraction;
    
    private readonly List<Interaction> interactions = new();
    
    private readonly CoordinatesConverter coordinatesConverter;

    public event Action<Interaction> OnHoveredInteractionChanged;

    public InteractionSystem(CoordinatesConverter coordinatesConverter)
    {
        this.coordinatesConverter = coordinatesConverter;
    }
    
    public void RegisterInteraction(Interaction interaction)
    {
        if (interactions.Contains(interaction))
            return;
        
        interactions.Add(interaction);
    }

    public void UnregisterInteraction(Interaction interaction)
    {
        if (interaction == hoveredInteraction)
        {
            hoveredInteraction?.OnHoverExit();
            hoveredInteraction = null;
            OnHoveredInteractionChanged?.Invoke(null);
        }

        if (interaction == pressedInteraction)
        {
            pressedInteraction = null;
        }

        interactions.Remove(interaction);
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
            if (interaction.Area.Contains(mousePosition))
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
        OnHoveredInteractionChanged?.Invoke(null);
    }
}