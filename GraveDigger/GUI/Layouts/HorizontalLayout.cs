using System.Collections.Generic;
using GraveDigger.GUI.Elements;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Layouts;

public class HorizontalLayout
{
    private Rectangle bounds;
    
    private List<UIElement> elements = new();
    
    public int PositionY { get; set; }
    public int Padding { get; set; }
    
    public HorizontalLayout(Rectangle bounds)
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
        int width = Padding * (elements.Count - 1);
        foreach (UIElement element in elements)
        {
            width += element.Bounds.Width;
        }
        
        int x = (int) (bounds.X + bounds.Width * 0.5f - width * 0.5f);
        
        foreach (UIElement element in elements)
        {
            element.SetPosition(x, PositionY);
            x += element.Bounds.Width + Padding;
        }
    }
}