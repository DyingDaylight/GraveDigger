using System;
using System.Linq;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Layouts;

public class HorizontalLayout : Layout
{

    public enum VerticalAlignment
    {
        UpperCenter,
        MiddleCenter,
        LowerCenter
    } 
    
    public VerticalAlignment Alignment { get; set; } = VerticalAlignment.UpperCenter; 
    
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
        var visibleElements = elements
            .Where(element => element.Visible)
            .ToList();
        
        if (visibleElements.Count == 0)
            return;
        
        int spacerCount = 0;
        int contentWidth = 0;
        
        // Calculate the total width of all visible elements
        // and count the spacers that will share the remaining space.
        foreach (ILayoutElement element in visibleElements)
        {
            if (element is ISpacer)
                spacerCount++;
            else
                contentWidth += (int) element.Size.X;
        }
        
        // Padding is added between every pair of layout elements.
        contentWidth += HorizontalPadding * (visibleElements.Count - 1);
        
        Rectangle contentBounds = GetContentBounds();
        
        // Calculate how much horizontal space remains after
        // subtracting the elements, padding, and margins.
        int freeSpace = contentBounds.Width - contentWidth;
        
        // Prevent spacers from receiving a negative size
        // when the content is wider than the available area.
        int spacerSize = spacerCount > 0 ? Math.Max(0, freeSpace / spacerCount) : 0;
        
        int totalWidth = contentWidth + spacerSize * spacerCount;
        
        // Center the complete layout inside the area between the margins.
        int x = (int) (contentBounds.X 
                       + (contentBounds.Width - totalWidth) * 0.5f);
        
        for (int i = 0; i < visibleElements.Count; i++)
        {
            ILayoutElement element = visibleElements[i];
            
            if (element is ISpacer)
            {
                x += spacerSize;
            }
            else
            {
                int elementY = contentBounds.Y;
                switch (Alignment)
                {
                    case VerticalAlignment.UpperCenter:
                        elementY = contentBounds.Y; 
                        break;
                    case VerticalAlignment.MiddleCenter:
                        elementY = (int) (contentBounds.Y + (contentBounds.Height - element.Size.Y) / 2);
                        break;
                    case VerticalAlignment.LowerCenter:
                        elementY = contentBounds.Bottom - (int)element.Size.Y;
                        break;
                }
                element.SetPosition(x, elementY);
                x += (int) element.Size.X;
            }
            
            // Add padding after every element except the last one.
            if (i < visibleElements.Count - 1)
                x += HorizontalPadding;
        }
    }
}