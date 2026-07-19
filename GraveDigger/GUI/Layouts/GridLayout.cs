
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

    public override void AddElement(ILayoutElement element)
    {
        base.AddElement(element);
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
        
        int cellWidth = (int) elements.Max(e => e.Size.X);
        int cellHeight = (int) elements.Max(e => e.Size.Y);
        
        // Calculate the total size of the grid,
        // including padding between cells.
        int contentWidth = cellWidth * columns + HorizontalPadding * (columns - 1);
        int contentHeight =  cellHeight * rows + VerticalPadding * (rows - 1);

        Rectangle contentBounds = GetContentBounds();
        
        int startX = (int) (contentBounds.X + (contentBounds.Width - contentWidth) * 0.5f);
        int startY = (int) (contentBounds.Y + (contentBounds.Height - contentHeight) * 0.5f);
        
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
                int elementX = cellX + (int)((cellWidth - element.Size.X) * 0.5f);
                int elementY = cellY + (int)((cellHeight - element.Size.Y) * 0.5f);
                
                element.SetPosition(elementX, elementY);
            }
        }
    }
}