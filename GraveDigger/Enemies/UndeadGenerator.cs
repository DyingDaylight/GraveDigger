using System.Collections.Generic;
using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Utils;

namespace GraveDigger.Enemies;

public class UndeadGenerator
{
    private static readonly Dictionary<Personality, (int None, int Ghost, int Zombie)> enemyRanges = new()
    {
        [Personality.Peaceful]   = (65, 20, 15),
        [Personality.Greedy]     = (25, 25, 50),
        [Personality.Bitter]     = (30, 30, 40),
        [Personality.Cruel]      = (10, 15, 75),
        [Personality.Mysterious] = (25, 45, 30),
        [Personality.Restless]   = (30, 45, 25),
    };
    
    public static EnemyType Generate(GraveSiteData graveSite, RandomService random)
    {
        var chances = enemyRanges[graveSite.Personality];

        int roll = random.Next(1, 101);

        if (roll <= chances.None)
            return EnemyType.None;

        if (roll <= chances.None + chances.Ghost)
            return EnemyType.Ghost;

        return EnemyType.Zombie;
    }
}