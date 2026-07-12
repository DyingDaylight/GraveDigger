using System;
using System.Collections.Generic;
using Interfaces;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GraveDigger.GUI.Layouts;

public class VerticalLayout : Layout
{

    public VerticalLayout(Rectangle bounds) : base(bounds)
    {
    }

    public void AddElement(ILayoutElement element)
    {
        if (element != null && !elements.Contains(element))
            elements.Add(element);
    }

    public override void UpdateLayout()
    {
        if (elements.Count == 0)
            return;
        
        int contentHeight = VerticalPadding * (elements.Count - 1);
        foreach (ILayoutElement element in elements)
            contentHeight += (int) element.VisibleSize.Y;
        
        int y = (int) (bounds.Y + (bounds.Height - contentHeight) * 0.5f);
        
        foreach (ILayoutElement element in elements)
        {
            int x = (int) (bounds.Center.X - element.VisibleSize.X * 0.5f);
            element.SetPosition(x, y);
            
            y += (int) element.VisibleSize.Y + VerticalPadding;
        }
    }
    
    public override void SetPosition(int x, int y)
    {
        base.SetPosition(x, y);
        UpdateLayout();
    }
}