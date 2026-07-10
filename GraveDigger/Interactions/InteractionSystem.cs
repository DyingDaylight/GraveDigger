using System.Collections.Generic;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GraveDigger.Interactions;

public class InteractionSystem : IUpdatable
{
    private Interaction hoveredInteraction;
    private MouseState previousMouseState;
    private Interaction pressedInteraction;
    
    private List<Interaction> interactions = new List<Interaction>();
    
    private Camera camera;

    public InteractionSystem(Camera camera)
    {
        this.camera = camera;
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
        Matrix inverseViewMatrix = Matrix.Invert(camera.TransformMatrix);
        Vector2 mousePosition = Vector2.Transform(screenMouse, inverseViewMatrix);
        
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
}