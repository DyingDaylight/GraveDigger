using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GraveDigger.Utils;

public class LootPlacementService
{
    private const int SearchWidth = 360;
    private const int SearchHeight = 220;
    private const int StartOffsetY = 30;
    
    public static Vector2? FindFreePosition(Vector2 origin, Point itemSize, 
        IReadOnlyList<Rectangle> occupiedAreas, int padding = 10)
    {
        int stepX = itemSize.X + padding;
        int stepY = itemSize.Y + padding;

        int maxOffsetX = SearchWidth / 2;
        int maxOffsetY = SearchHeight;

        for (int offsetY = StartOffsetY; offsetY <= maxOffsetY; offsetY += stepY)
        {
            foreach (int offsetX in GetHorizontalOffsets(maxOffsetX, stepX))
            {
                Vector2 position = origin + new Vector2(offsetX, offsetY);

                Rectangle candidateBounds = CreateBounds(
                    position,
                    itemSize,
                    padding
                );

                bool isOccupied = occupiedAreas.Any(
                    area => candidateBounds.Intersects(area)
                );

                if (!isOccupied)
                    return position;
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