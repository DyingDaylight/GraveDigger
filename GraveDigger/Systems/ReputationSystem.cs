using System;

namespace GraveDigger.Systems;

public class ReputationSystem
{
    public int Value { get; private set; }

    public event Action<int> ReputationChanged;
    
    public void AddReputation(int value)
    {
        if (value < 0)
            return;
        
        Value += value;
        ReputationChanged?.Invoke(Value);
    }

    public void RemoveReputation(int value)
    {
        if (value <= 0)
            return;
        
        Value = Math.Max(0, Value - value);
        ReputationChanged?.Invoke(Value);
    }
}