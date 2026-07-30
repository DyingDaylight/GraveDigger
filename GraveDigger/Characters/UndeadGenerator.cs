using System;
using System.Collections.Generic;
using GraveDigger.Data;
using GraveDigger.Utils;

namespace GraveDigger.Enemies;

public class UndeadGenerator
{
    private static readonly Dictionary<Personality, float> GhostDayChances = new()
    {
        [Personality.Peaceful]   = 0,
        [Personality.Mysterious] = 0.01f,
        [Personality.Restless]   = 0.05f,
        [Personality.Greedy]     = 0.10f,
        [Personality.Bitter]     = 0.15f,
        [Personality.Cruel]      = 0.20f
    };
    
    private static readonly Dictionary<Personality, float> GhostNightChances = new()
    {
        [Personality.Peaceful]   = 0.01f,
        [Personality.Mysterious] = 0.04f,
        [Personality.Restless]   = 0.1f,
        [Personality.Greedy]     = 0.15f,
        [Personality.Bitter]     = 0.20f,
        [Personality.Cruel]      = 0.25f
    };

    public static EnemyType Generate(GraveSiteData graveSite, RandomService random, bool isNight)
    {
        float ghostChance = isNight ? GhostNightChances[graveSite.Personality] : GhostDayChances[graveSite.Personality];

        return random.Chance(ghostChance) ? EnemyType.Ghost : EnemyType.None;
    }
}