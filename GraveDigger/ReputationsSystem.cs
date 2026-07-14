using System;

namespace GraveDigger;

public class ReputationsSystem
{
    private int value = 0;

    public event Action<int> ReputationChanged;
    
    public void AddReputation(int value)
    {
        if (value < 0)
            return;
        
        this.value += value;
        ReputationChanged?.Invoke(this.value);
    }

    public void RemoveReputation(int value)
    {
        if (value > 0)
            return;
        
        this.value = Math.Max(0, this.value - value);
        ReputationChanged?.Invoke(this.value);
    }
}