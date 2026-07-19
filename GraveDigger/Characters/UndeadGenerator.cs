using System;
using System.Collections.Generic;
using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Utils;

namespace GraveDigger.Enemies;

public class UndeadGenerator
{
    private static readonly Dictionary<Personality, int> GhostChances = new()
    {
        [Personality.Peaceful]   = 10,
        [Personality.Mysterious] = 25,
        [Personality.Restless]   = 35,
        [Personality.Greedy]     = 45,
        [Personality.Bitter]     = 55,
        [Personality.Cruel]      = 70
    };

    private const int NightChanceBonus = 20;
    
    public static EnemyType Generate(GraveSiteData graveSite, RandomService random, bool isNight)
    {
        int ghostChance = GhostChances[graveSite.Personality];

        if (isNight)
            ghostChance += NightChanceBonus;

        ghostChance = Math.Clamp(ghostChance, 0, 100);

        int roll = random.Next(1, 101);

        return roll <= ghostChance
            ? EnemyType.Ghost
            : EnemyType.None;
    }
}