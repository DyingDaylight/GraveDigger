using System;
using System.Collections.Generic;
using GraveDigger.Props;

namespace GraveDigger.Systems;

public class ReputationSystem
{
    public const int MinValue = -100;
    public const int MaxValue = 100;
    
    public int Value { get; private set; }

    public event Action<int, int, int> ReputationChanged;
    
    public void ChangeReputation(int value)
    {
        int newValue = Math.Clamp(Value + value, MinValue, MaxValue);

        if (newValue == Value)
            return;

        Console.WriteLine($"Reputation changed to {Value} -> {newValue}");
        Value = newValue;
        ReputationChanged?.Invoke(Value,  MinValue, MaxValue);
    }

    public void Calculate(List<Prop> props)
    {
        int total = 0;

        foreach (Prop prop in props)
        {
            total += prop.GetReputationValue();
        }

        Console.WriteLine($"Initial reputation: {total}");
        ChangeReputation(total);
    }
}