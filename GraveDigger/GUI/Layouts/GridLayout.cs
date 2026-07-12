
using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Layouts;

public class GridLayout : Layout
{
    private Vector2 paddings = Vector2.Zero;
    private int columns;
    private int rows;

    private List<IUISizable> elements = new();

    public GridLayout(Rectangle bounds) : base(bounds)
    {
    }
    
    public void SetColumns(int columns)
    {
        this.columns = columns;
    }

    public void SetRows(int rows)
    {
        this.rows = rows;
    }

    public void SetPadding(Vector2 vector2)
    {
        paddings = vector2;
    }

    public void AddElement(IUISizable element)
    {
        elements.Add(element);
    }

    public void UpdateLayout()
    {
        if (elements.Count == 0 || columns <= 0)
            return;
        
        int slotWidth = (int) elements.Max(e => e.VisibleSize.X);
        int slotHeight = (int) elements.Max(e => e.VisibleSize.Y);
        
        int horizontalPadding = (int) paddings.X;
        int verticalPadding = (int) paddings.Y;
        
        // Calculate the total size of the grid,
        // including padding between cells.
        int contentWidth = slotWidth * columns + horizontalPadding * (columns - 1);
        int contentHeight =  slotHeight * rows + verticalPadding * (rows - 1);

        int startX = (int) (bounds.X + (bounds.Width - contentWidth) * 0.5f);
        int startY = (int) (bounds.Y + (bounds.Height - contentHeight) * 0.5f);
        
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int index = (row * columns) + column;
                
                if (index >= elements.Count)
                    break;
                
                IUISizable element = elements[index];

                int cellX = startX + column * (slotWidth + horizontalPadding);
                int cellY = startY + row * (slotHeight + verticalPadding);
                
                // Center the element inside its grid cell.
                int elementX = cellX + (int)((slotWidth - element.VisibleSize.X) * 0.5f);
                int elementY = cellY + (int)((slotHeight - element.VisibleSize.Y) * 0.5f);
                
                element.SetPosition(elementX, elementY);
            }
        }
    }
}