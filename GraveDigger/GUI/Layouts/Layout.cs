using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Layouts;

public class Layout : IUISizable
{
    protected Rectangle bounds;
    
    public virtual Vector2 VisibleSize => new(bounds.Width, bounds.Height);
    public bool IsSpacer => false;

    public Layout(Rectangle bounds)
    {
        this.bounds = bounds;
    }
    
    public virtual void SetPosition(int x, int y)
    {
        bounds.X = x;
        bounds.Y = y;
    }
}