using System;
using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GraveDigger.GUI.Components;

public class NotificationPopup : Tooltip
{
    private const int TopMargin = 50;
    private readonly Rectangle parentBounds;
    private float remainingTime;

    public NotificationPopup(Rectangle parentBounds)
    {
        this.parentBounds = parentBounds;
        Visible = false;
    }

    public void Show(string text, float duration = 3f)
    {
        SetTooltip(text);
        
        CenterAtTop();
        
        remainingTime = duration;
        Visible = true;
    }

    public override void Update(GameTime gameTime)
    {
        if (!Visible)
            return;

        base.Update(gameTime);
        
        remainingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (remainingTime <= 0)
        {
            Visible = false;
            remainingTime = 0;
        }
    }
    
    private void CenterAtTop()
    {
        int x = (int) (parentBounds.X + (parentBounds.Width - Bounds.Width) * 0.5f);
        int y = parentBounds.Y + TopMargin;

        SetPosition(x, y);
    }
}