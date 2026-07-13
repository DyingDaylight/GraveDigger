using GraveDigger.GUI.Elements;
using GraveDigger.Interactions;
using GraveDigger.Utils;
using Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IDrawable = Interfaces.IDrawable;

namespace GraveDigger.GUI.Components;

public class InteractionTooltip : IUpdatable, IDrawable
{
    private readonly CoordinatesConverter coordinatesConverter;
    private readonly Tooltip tooltip = new();
    
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
        if (currentInteraction == null)
            return;
        
        UpdatePosition();
        tooltip.Update(gameTime);
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (currentInteraction == null)
            return;
        
        tooltip.Draw(spriteBatch);
    }
    
    public void SetInteraction(Interaction interaction)
    {
        currentInteraction = interaction;
        
        if (currentInteraction != null)
            tooltip.SetTooltip(currentInteraction.Hint);
    }

    public void ClearInteraction()
    {
        currentInteraction = null;
    }

    private void UpdatePosition()
    {
        Vector2 screenAnchor = coordinatesConverter.WorldToScreen(new Vector2(
            currentInteraction.Area.Center.X, currentInteraction.Area.Top));
        
        int x = (int)(screenAnchor.X - tooltip.Bounds.Width * 0.5f);
        int y = (int) (screenAnchor.Y - tooltip.Bounds.Height - 20);
        
        tooltip.SetPosition(x, y);
    }
}