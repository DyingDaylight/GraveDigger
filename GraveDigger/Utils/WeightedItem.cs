namespace GraveDigger.Utils;

public class WeightedItem<T>
{
    public T Value { get; }
    public int Weight { get; }

    public WeightedItem(T value, int weight)
    {
        Value = value;
        Weight = weight;
    }
}