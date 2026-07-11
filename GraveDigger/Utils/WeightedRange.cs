namespace GraveDigger.Utils;

public class WeightedRange
{
    public int Min { get; }
    public int Max { get; }
    public int Weight { get; }

    public WeightedRange(int min, int max, int weight)
    {
        Min = min;
        Max = max;
        Weight = weight;
    }
}