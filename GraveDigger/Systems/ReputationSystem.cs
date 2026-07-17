using System;

namespace GraveDigger.Systems;

public class ReputationSystem
{
    public const int MinValue = -100;
    public const int MaxValue = 100;
    
    public int Value { get; private set; }

    public event Action<int, int, int> ReputationChanged;
    
    public void AddReputation(int value)
    {
        if (value <= 0)
            return;

        ChangeReputation(value);
    }

    public void RemoveReputation(int value)
    {
        if (value <= 0)
            return;

        ChangeReputation(-value);
    }
    
    private void ChangeReputation(int value)
    {
        int newValue = Math.Clamp(Value + value, MinValue, MaxValue);

        if (newValue == Value)
            return;

        Value = newValue;
        ReputationChanged?.Invoke(Value,  MinValue, MaxValue);
    }
}