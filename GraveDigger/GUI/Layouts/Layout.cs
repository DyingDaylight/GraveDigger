using System;
using System.Collections.Generic;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Layouts;

public abstract class Layout : ILayoutElement
{
    protected readonly List<ILayoutElement> elements = new();
    
    protected Rectangle bounds;

    public Vector2 Margins { get; set; }
    public int HorizontalPadding { get; set; }
    public int VerticalPadding { get; set; }
    
    public virtual Vector2 Size => new(bounds.Width, bounds.Height);
    public bool Visible { get; } = true;


    protected Layout(Rectangle bounds)
    {
        this.bounds = bounds;
    }
    
    public virtual void AddElement(ILayoutElement element)
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

    public virtual void SetBounds(Rectangle bounds)
    {
        this.bounds = bounds;
    }
    
    public abstract void UpdateLayout();
    
    public virtual void RemoveAllElements()
    {
        elements.Clear();
    }
    
    protected Rectangle GetContentBounds()
    {
        return new Rectangle(
            bounds.X + (int)Margins.X,
            bounds.Y + (int)Margins.Y,
            Math.Max(0, bounds.Width - (int)Margins.X * 2),
            Math.Max(0, bounds.Height - (int)Margins.Y * 2));
    }
}