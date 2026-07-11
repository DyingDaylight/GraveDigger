namespace GraveDigger.Utils;

public class WeightedList<T>
{
    public T Value { get; }
    public int Weight { get; }

    public WeightedList(T value, int weight)
    {
        Value = value;
        Weight = weight;
    }
}