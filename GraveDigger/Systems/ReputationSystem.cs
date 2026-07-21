using System;
using System.Collections.Generic;
using Interfaces;

namespace GraveDigger.Systems;

public class ReputationSystem
{
    public const int MinValue = -100;
    public const int MaxValue = 100;
    
    public int Value { get; private set; }

    public event Action<int, int, int> ReputationChanged;
    
    public void Recalculate(IEnumerable<IReputationContributor> contributors)
    {
        int total = 0;

        foreach (IReputationContributor contributor in contributors)
            total += contributor.GetReputationValue();

        SetValue(total);
    }

    private void SetValue(int value)
    {
        int newValue = Math.Clamp(value, MinValue, MaxValue);

        if (newValue == Value)
            return;

        Console.WriteLine($"Reputation changed: {Value} -> {newValue}");

        Value = newValue;
        ReputationChanged?.Invoke(Value, MinValue, MaxValue);
    }
}