using System.Collections.Generic;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Layouts;

public abstract class Layout : ILayoutElement
{
    protected readonly List<ILayoutElement> elements = new();
    
    protected Rectangle bounds;
    
    public int HorizontalPadding { get; set; }
    public int VerticalPadding { get; set; }
    
    public virtual Vector2 VisibleSize => new(bounds.Width, bounds.Height);


    public Layout(Rectangle bounds)
    {
        this.bounds = bounds;
    }
    
    public void AddElement(ILayoutElement element)
    {
        if (element == null || elements.Contains(element))
            return;

        elements.Add(element);
    }
    
    public virtual void SetPosition(int x, int y)
    {
        bounds.X = x;
        bounds.Y = y;
    }
    
    public abstract void UpdateLayout();
}