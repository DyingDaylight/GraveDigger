using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GraveDigger.Utils;

public class LootPlacementService
{
    private static readonly Vector2[] Offsets =
    {
        new(0, 50),
        new(-45, 45),
        new(45, 45),
        new(-80, 65),
        new(80, 65),
        new(0, 90),
        new(-120, 85),
        new(120, 85)
    };
    
    public static Vector2? FindFreePosition(Vector2 origin, Point itemSize, 
        IReadOnlyList<Rectangle> occupiedAreas, int padding = 10)
    {
        foreach (Vector2 offset in Offsets)
        {
            Vector2 position = origin + offset;

            Rectangle candidateBounds = new(
                (int)position.X - padding,
                (int)position.Y - padding,
                itemSize.X + padding * 2,
                itemSize.Y + padding * 2
            );

            bool isOccupied = occupiedAreas.Any(area => candidateBounds.Intersects(area));

            if (!isOccupied)
                return position;
        }

        return null;
    }
}