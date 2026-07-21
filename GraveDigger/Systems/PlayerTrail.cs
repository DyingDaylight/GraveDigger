using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GraveDigger.Systems;

public class PlayerTrail
{
    private readonly List<Vector2> positions = new();

    // Minimum distance between recorded trail points.
    private const float PointSpacing = 15f;
    // Number of trail points separating each follower.
    private const int PointsPerFollower = 8;
    private const int MaxFollowers = 10;
    private const int MaxPoints = (MaxFollowers + 1) * PointsPerFollower + 20;
    
    public void Record(Vector2 position)
    {
        if (positions.Count == 0)
        {
            positions.Add(position);
            return;
        }

        Vector2 lastPosition = positions[^1];

        if (Vector2.Distance(lastPosition, position) < PointSpacing)
            return;

        positions.Add(position);

        if (positions.Count > MaxPoints)
            positions.RemoveAt(0);
    }

    public Vector2 GetFollowerPosition(int followerIndex)
    {
        if (positions.Count == 0)
            return Vector2.Zero;

        // Each follower stays several recorded points behind the previous one.
        int pointsBehind = (followerIndex + 1) * PointsPerFollower;
        int targetIndex = positions.Count - 1 - pointsBehind;

        targetIndex = Math.Max(targetIndex, 0);

        return positions[targetIndex];
    }
}