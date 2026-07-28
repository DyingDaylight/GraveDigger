using System;

namespace GraveDigger.Utils;

public static class BurialChanceCalculator
{
    // 65% base chance, ±35% depending on reputation.
    private const float BaseChance = 0.65f;
    private const float ReputationModifier = 0.0035f;
    private const float MinChance = 0.3f;
    private const float MaxChance = 1f;

    public static float Calculate(int reputation)
    {
        float chance = BaseChance + reputation * ReputationModifier;
        
        return Math.Clamp(chance, MinChance, MaxChance);
    }
}
