using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GraveDigger.Utils;

public class LootPlacementService
{
    public readonly record struct LootPlacement(
        Vector2 Position,
        Rectangle Bounds
    );
    
    private const int SearchWidth = 700;
    private const int SearchHeight = 700;
    private const int SearchStep = 20;
    private const int StartOffsetY = 30;

    public static LootPlacement? FindFreePosition(
        Vector2 origin,
        Point itemSize,
        IReadOnlyList<Rectangle> occupiedAreas,
        int padding = 10)
    {
        int maxOffsetX = SearchWidth / 2;
        int startOffsetY = itemSize.Y / 2 + padding;

        for (int offsetY = startOffsetY;
             offsetY <= SearchHeight;
             offsetY += SearchStep)
        {
            foreach (int offsetX in GetHorizontalOffsets(maxOffsetX, SearchStep))
            {
                Vector2 position = origin + new Vector2(offsetX, offsetY);
                Rectangle bounds = CreateBounds(position, itemSize, padding);

                bool isOccupied = occupiedAreas.Any(
                    area => bounds.Intersects(area));

                if (!isOccupied)
                    return new LootPlacement(position, bounds);
            }
        }

        return null;
    }
    
    private static IEnumerable<int> GetHorizontalOffsets(
        int maxOffset,
        int step)
    {
        yield return 0;

        for (int offset = step; offset <= maxOffset; offset += step)
        {
            yield return -offset;
            yield return offset;
        }
    }

    private static Rectangle CreateBounds(
        Vector2 position,
        Point itemSize,
        int padding)
    {
        return new Rectangle(
            (int)(position.X - itemSize.X * 0.5f) - padding,
            (int)(position.Y - itemSize.Y * 0.5f) - padding,
            itemSize.X + padding * 2,
            itemSize.Y + padding * 2
        );
    }
}