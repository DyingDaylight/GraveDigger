using System;
using System.Collections.Generic;
using GraveDigger.Data;
using GraveDigger.Utils;

namespace GraveDigger.Enemies;

public class UndeadGenerator
{
    private static readonly Dictionary<Personality, int> GhostChances = new()
    {
        [Personality.Peaceful]   = 1,
        [Personality.Mysterious] = 2,
        [Personality.Restless]   = 3,
        [Personality.Greedy]     = 6,
        [Personality.Bitter]     = 8,
        [Personality.Cruel]      = 12
    };

    public static EnemyType Generate(GraveSiteData graveSite, RandomService random, bool isNight)
    {
        int ghostChance = GhostChances[graveSite.Personality];
        
        bool spawned = random.Chance(ghostChance);
        if (isNight && !spawned)
        {
            spawned = random.Chance(ghostChance);
        }
        
        return spawned
            ? EnemyType.Ghost
            : EnemyType.None;
    }
}