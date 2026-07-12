using System;
using System.Collections.Generic;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Layouts;

public class HorizontalLayout : Layout
{
    // X is the left margin, Y is the right margin.
    public Vector2 HorizontalMargins { get; set; }
    
    public HorizontalLayout(Rectangle bounds) : base(bounds)
    {
    }
    
    public override void SetPosition(int x, int y)
    {
        base.SetPosition(x, y);
        UpdateLayout();
    }
    
    public override void UpdateLayout()
    {
        if (elements.Count == 0)
            return;
        
        int spacerCount = 0;
        int contentWidth = 0;
        
        // Calculate the total width of all visible elements
        // and count the spacers that will share the remaining space.
        foreach (ILayoutElement element in elements)
        {
            if (element is ISpacer)
                spacerCount++;
            else
                contentWidth += (int) element.VisibleSize.X;
        }

        // Padding is added between every pair of layout elements.
        contentWidth += HorizontalPadding * (elements.Count - 1);
        
        int marginsWidth = (int) (HorizontalMargins.X + HorizontalMargins.Y);
        
        // Calculate how much horizontal space remains after
        // subtracting the elements, padding, and margins.
        int freeSpace = bounds.Width - marginsWidth - contentWidth;
        
        // Prevent spacers from receiving a negative size
        // when the content is wider than the available area.
        int spacerSize = spacerCount > 0 ? Math.Max(0, freeSpace / spacerCount) : 0;
        
        int totalWidth = contentWidth + spacerSize * spacerCount;
        
        // Center the complete layout inside the area between the margins.
        int x = (int) (bounds.X 
                       + HorizontalMargins.X
                       + (bounds.Width - marginsWidth - totalWidth) * 0.5f);
        
        for (int i = 0; i < elements.Count; i++)
        {
            ILayoutElement element = elements[i];
            
            if (element is ISpacer)
            {
                x += spacerSize;
            }
            else
            {
                element.SetPosition(x, bounds.Y);
                x += (int) element.VisibleSize.X;
            }
            
            // Add padding after every element except the last one.
            if (i < elements.Count - 1)
                x += HorizontalPadding;
        }
    }
}