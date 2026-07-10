using System.Collections.Generic;
using System.Drawing;
using GraveDigger.GUI.Elements;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GraveDigger.GUI.Layouts;

public class VerticalLayout
{
    private Rectangle bounds;
    
    private List<UIElement> elements = new();
    
    public int Padding { get; set; }
    
    public VerticalLayout(Rectangle bounds)
    {
        this.bounds = bounds;
    }

    public void AddElement(UIElement button)
    {
        if (button != null && !elements.Contains(button))
            elements.Add(button);
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
            int X = (int) (centerX - element.Bounds.Width * 0.5f);
            element.SetPosition(X, Y);
            Y += element.Bounds.Height + Padding;
        }
    }
}