using System;
using System.Collections.Generic;
using System.Linq;

namespace GraveDigger.Utils;

public class RandomService
{
    private readonly Random random;
    
    public RandomService(int seed)
    {
        random = new Random(seed);
    }

    public int Next(int minInclusive, int maxExclusive)
    {
        return random.Next(minInclusive, maxExclusive);
    }

    public float NextFloat()
    {
        return (float)random.NextDouble();
    }

    public bool Chance(float probability)
    {
        return NextFloat() < probability;
    }

    public T RandomEnum<T>() where T : struct, Enum
    {
        T[] values = Enum.GetValues<T>();
        return values[Next(0, values.Length)];
    }

    public T Pick<T>(IReadOnlyList<T> items)
    {
        return items[Next(0, items.Count)];
    }
    
    public int PickWeightedRange(IReadOnlyList<WeightedRange> ranges)
    {
        int totalWeight = ranges.Sum(r => r.Weight);

        int roll = Next(0, totalWeight);

        foreach (WeightedRange range in ranges)
        {
            if (roll < range.Weight)
            {
                return Next(range.Min, range.Max + 1);
            }

            roll -= range.Weight;
        }

        throw new InvalidOperationException();
    }
}