using System.Collections.Generic;
using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Utils;

namespace GraveDigger.Enemies;

public class UndeadGenerator
{
    private static readonly Dictionary<TombPersonality, (int None, int Ghost, int Zombie)> enemyRanges = new()
    {
        [TombPersonality.Peaceful]   = (65, 20, 15),
        [TombPersonality.Greedy]     = (25, 25, 50),
        [TombPersonality.Bitter]     = (30, 30, 40),
        [TombPersonality.Cruel]      = (10, 15, 75),
        [TombPersonality.Mysterious] = (25, 45, 30),
        [TombPersonality.Restless]   = (30, 45, 25),
    };
    
    public static EnemyType Generate(TombstoneData tombstone, RandomService random)
    {
        var chances = enemyRanges[tombstone.Personality];

        int roll = random.Next(1, 101);

        if (roll <= chances.None)
            return EnemyType.None;

        if (roll <= chances.None + chances.Ghost)
            return EnemyType.Ghost;

        return EnemyType.Zombie;
    }
}