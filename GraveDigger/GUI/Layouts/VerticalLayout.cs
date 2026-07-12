using System.Collections.Generic;
using System.Drawing;
using GraveDigger.GUI.Elements;
using Interfaces;
using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GraveDigger.GUI.Layouts;

public class VerticalLayout : Layout
{
    private List<UIElement> elements = new();
    
    public override Vector2 VisibleSize => new(bounds.Width, bounds.Height);
    public int Padding { get; set; }

    public VerticalLayout(Rectangle bounds) : base(bounds)
    {
    }

    public void AddElement(UIElement element)
    {
        if (element != null && !elements.Contains(element))
            elements.Add(element);
    }
    
    public void CountPositions()
    {
        float centerX = bounds.X + bounds.Width * 0.5f;
        float centerY = bounds.Y + bounds.Height * 0.5f;
        
        int height = Padding * (elements.Count - 1);
        foreach (UIElement element in elements)
        {
            height += element.Bounds.Height;
        }

        int Y = (int) (bounds.Y + bounds.Height * 0.5f - height * 0.5f);
        foreach (UIElement element in elements)
        {
            int X = (int) (centerX - element.VisibleSize.X * 0.5f);
            element.SetPosition(X, Y);
            Y += (int) element.VisibleSize.Y + Padding;
        }
    }
    
    public override void SetPosition(int x, int y)
    {
        bounds.X = x;
        bounds.Y = y;
    }
}