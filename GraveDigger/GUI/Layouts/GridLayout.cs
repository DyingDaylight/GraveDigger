
using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.GUI.Layouts;

public class GridLayout : Layout
{
    private int columns;
    private int rows;
    
    public GridLayout(Rectangle bounds) : base(bounds)
    {
    }
    
    public void SetColumns(int columns)
    {
        if (columns <= 0)
            throw new ArgumentOutOfRangeException(nameof(columns));
        
        this.columns = columns;
        UpdateLayout();
    }

    public void SetRows(int rows)
    {
        if (rows <= 0)
            throw new ArgumentOutOfRangeException(nameof(rows));
        
        this.rows = rows;
        UpdateLayout();
    }

    public void SetPadding(int horizontalPadding, int verticalPadding)
    {
        HorizontalPadding = horizontalPadding;
        VerticalPadding = verticalPadding;
        UpdateLayout();
    }

    public void AddElement(ILayoutElement element)
    {
        if (element == null || elements.Contains(element))
            return;
        
        elements.Add(element);
        UpdateLayout();
    }

    public override void UpdateLayout()
    {
        if (elements.Count == 0 || columns <= 0 || rows <= 0)
            return;
        
        if (elements.Count > rows * columns)
        {
            throw new InvalidOperationException(
                "The grid does not have enough cells for all elements.");
        }
        
        int cellWidth = (int) elements.Max(e => e.VisibleSize.X);
        int cellHeight = (int) elements.Max(e => e.VisibleSize.Y);
        
        // Calculate the total size of the grid,
        // including padding between cells.
        int contentWidth = cellWidth * columns + HorizontalPadding * (columns - 1);
        int contentHeight =  cellHeight * rows + VerticalPadding * (rows - 1);

        int startX = (int) (bounds.X + (bounds.Width - contentWidth) * 0.5f);
        int startY = (int) (bounds.Y + (bounds.Height - contentHeight) * 0.5f);
        
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int index = (row * columns) + column;
                
                if (index >= elements.Count)
                    break;
                
                ILayoutElement element = elements[index];

                int cellX = startX + column * (cellWidth + HorizontalPadding);
                int cellY = startY + row * (cellHeight + VerticalPadding);
                
                // Center the element inside its grid cell.
                int elementX = cellX + (int)((cellWidth - element.VisibleSize.X) * 0.5f);
                int elementY = cellY + (int)((cellHeight - element.VisibleSize.Y) * 0.5f);
                
                element.SetPosition(elementX, elementY);
            }
        }
    }
}