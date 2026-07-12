using System;
using System.Collections.Generic;
using GraveDigger.GUI.Elements;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Layouts;

public class HorizontalLayout : Layout
{
    private List<IUISizable> elements = new();
    
    public int PositionY { get; set; }
    public int Padding { get; set; }
    
    public Vector2 horizontalMargins { get; set; }
    
    public HorizontalLayout(Rectangle bounds) : base(bounds)
    {
    }

    public void AddElement(IUISizable element)
    {
        if (element != null && !elements.Contains(element))
            elements.Add(element);
    }
    
    public override void SetPosition(int x, int y)
    {
        PositionY = y;
    }
    
    public void UpdateLayout()
    {
        if (elements.Count == 0)
            return;
        
        int spacerCount = 0;
        int contentWidth = 0;
        
        // Calculate the total width of all visible elements
        // and count the spacers that will share the remaining space.
        foreach (IUISizable element in elements)
        {
            if (element.IsSpacer)
                spacerCount++;
            else
                contentWidth += (int) element.VisibleSize.X;
        }

        // Padding is added between every pair of layout elements.
        contentWidth += Padding * (elements.Count - 1);
        
        int marginsWidth = (int) (horizontalMargins.X + horizontalMargins.Y);
        
        // Calculate how much horizontal space remains after
        // subtracting the elements, padding, and margins.
        int freeSpace = bounds.Width - marginsWidth - contentWidth;
        
        // Prevent spacers from receiving a negative size
        // when the content is wider than the available area.
        int spacerSize = spacerCount > 0 ? Math.Max(0, freeSpace / spacerCount) : 0;
        
        int totalWidth = contentWidth + spacerSize * spacerCount;
        
        // Center the complete layout inside the area between the margins.
        int x = (int) (bounds.X 
                       + horizontalMargins.X
                       + (bounds.Width - marginsWidth - totalWidth) * 0.5f 
                       );
        
        for (int i = 0; i < elements.Count; i++)
        {
            IUISizable element = elements[i];
            
            if (element.IsSpacer)
            {
                x += spacerSize;
            }
            else
            {
                element.SetPosition(x, PositionY);
                x += (int)element.VisibleSize.X;
            }
            
            // Add padding after every element except the last one.
            if (i < elements.Count - 1)
                x += Padding;
        }
    }
}