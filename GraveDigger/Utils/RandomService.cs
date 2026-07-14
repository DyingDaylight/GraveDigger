using System;
using System.Collections.Generic;
using System.Linq;

namespace GraveDigger.Utils;

public class RandomService
{
    private readonly Random random;
    
    public RandomService(int? seed = null)
    {
        if (seed.HasValue)
            random = new Random(seed.Value);
        else
            random = new Random();
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
        probability = Math.Clamp(probability, 0f, 1f);
        return NextFloat() < probability;
    }

    public T RandomEnum<T>() where T : struct, Enum
    {
        T[] values = Enum.GetValues<T>();
        return Pick(values);
    }

    public T Pick<T>(IReadOnlyList<T> items)
    {
        if (items.Count == 0)
            throw new ArgumentException("Collection cannot be empty.");
        
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