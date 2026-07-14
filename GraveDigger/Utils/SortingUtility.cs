using System;
using Microsoft.Xna.Framework;

namespace GraveDigger.Utils;

public class SortingUtility
{
    private static float worldHeight;

    public static void Initialize(float worldHeight)
    {
        if (worldHeight <= 0)
            throw new ArgumentOutOfRangeException(nameof(worldHeight));

        SortingUtility.worldHeight = worldHeight;
    }

    public static float CalculateByY(float bottom)
    {
        if (worldHeight <= 0)
            throw new InvalidOperationException(
                "SortingUtility must be initialized before use.");

        float depth = bottom / worldHeight;
        return 1f - MathHelper.Clamp(depth, 0f, 1f);
    }
}