using System;

namespace GraveDigger.Utils;

public static class BurialChanceCalculator
{
    // 80% base chance, ±20% depending on reputation.
    private const float BaseChance = 0.8f;
    private const float ReputationModifier = 0.002f;
    private const float MinChance = 0.6f;
    private const float MaxChance = 1f;

    public static float Calculate(int reputation)
    {
        float chance = BaseChance + reputation * ReputationModifier;
        
        return Math.Clamp(chance, MinChance, MaxChance);
    }
}
