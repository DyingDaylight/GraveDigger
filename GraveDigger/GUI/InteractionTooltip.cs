using System;
using System.Diagnostics;
using GraveDigger;
using GraveDigger.GUI.Elements;
using GraveDigger.Interactions;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GUI;

public class InteractionTooltip : IUpdatable, IDrawable
{
    private CoordinatesConverter coordinatesConverter;
    
    private Tooltip tooltip = new Tooltip();
    private Interaction currentInteraction;

    public InteractionTooltip(CoordinatesConverter coordinatesConverter)
    {
        this.coordinatesConverter = coordinatesConverter;
    }
    
    public void Start()
    {
        tooltip.Start();
    }

    public void Update(GameTime gameTime)
    {
        if (currentInteraction != null)
        {
            UpdateTooltip();
            tooltip.Update(gameTime);
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (currentInteraction != null)
            tooltip.Draw(spriteBatch);
    }
    
    public void HandleInteraction(Interaction interaction)
    {
        currentInteraction = interaction;
    }

    private void UpdateTooltip()
    {
        if (currentInteraction == null)
            return;
        
        tooltip.SetTooltip(currentInteraction.Hint);
        
        int x = (int)(currentInteraction.GetArea().X + currentInteraction.GetArea().Width * 0.5f - tooltip.Bounds.Width * 0.5f);
        int y = currentInteraction.GetArea().Y - tooltip.Bounds.Height - 20;
        
        Vector2 screenPosition = coordinatesConverter.WorldToScreen(new Vector2(x, y));
        tooltip.SetPosition((int) screenPosition.X, (int) screenPosition.Y);
    }
}