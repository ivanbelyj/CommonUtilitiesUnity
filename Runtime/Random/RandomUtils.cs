using System.Collections.Generic;
using UnityEngine;

public static class RandomExtensions
{
    /// <summary>
    /// Returns a random double between 0.0 and 1.0 (inclusive)
    /// </summary>
    public static double NextDoubleInclusive(this System.Random random)
    {
        return random.Next() / (double)(int.MaxValue - 1);
    }
}

public static class IEnumerableRandomExtensions
{
    public static T GetRandomOne<T>(this IList<T> items, System.Random random = null)
        => RandomUtils.GetRandomOne(items, random);
}

public static class IReadOnlyListExtensions
{
    public static T GetRandomWeighted<T>(
        this IReadOnlyList<T> items,
        System.Func<T, int, float> getWeight,
        System.Random random = null)
        => RandomUtils.GetRandomWeighted(items, getWeight, random);

    public static T GetRandomWeighted<T>(
        this IReadOnlyList<T> items,
        System.Func<T, float> getWeight,
        System.Random random = null)
        => RandomUtils.GetRandomWeighted(items, (x, _) => getWeight(x), random);
}

public static class RandomUtils
{
    /// <summary>
    /// Selects a random element from the list with equal probability for each item.
    /// </summary>
    /// <returns>A randomly selected item.</returns>
    public static T GetRandomOne<T>(IList<T> items, System.Random random)
        => items[(int)((random == null ? Random.value : random.NextDoubleInclusive()) * items.Count)];

    /// <summary>
    /// Selects a random element from the list using weighted probabilities.
    /// </summary>
    /// <param name="items">The list of items to choose from.</param>
    /// <param name="getWeight">Function that returns weight for each item (item, index).</param>
    /// <param name="random">Optional random generator.</param>
    /// <returns>A randomly selected item based on the provided weights.</returns>
    public static T GetRandomWeighted<T>(
        IReadOnlyList<T> items,
        System.Func<T, int, float> getWeight,
        System.Random random = null)
    {
        if (items == null || getWeight == null)
        {
            throw new System.ArgumentException("Items and getWeight must be non-null.");
        }

        if (items.Count == 0)
        {
            throw new System.ArgumentException("Items list must be non-empty.");
        }

        T selected = default;
        float total = 0f;
        bool hasPositiveWeight = false;

        for (int i = 0; i < items.Count; i++)
        {
            float weight = getWeight(items[i], i);
            if (weight < 0)
            {
                throw new System.ArgumentException($"Weight must be non-negative (got {weight} for item at index {i}).");
            }

            total += weight;

            if (weight > 0)
            {
                hasPositiveWeight = true;
                float r = (random == null
                    ? Random.value
                    : (float)random.NextDoubleInclusive()) * total;

                if (r <= weight)
                {
                    selected = items[i];
                }
            }
        }

        if (!hasPositiveWeight)
        {
            throw new System.ArgumentException("At least one weight must be positive.");
        }

        return selected;
    }

    [System.Obsolete("Please, use other overload that accepts delegate")]
    public static T GetRandomWeighted<T>(IReadOnlyList<T> items, IReadOnlyList<float> weights, System.Random random = null)
    {
        return GetRandomWeighted(items, (_, index) => weights[index], random);
    }

    /// <summary>
    /// Box-Muller transform for normal distribution
    /// </summary>
    public static float NormalDistribution(float min, float max, System.Random random)
    {
        float mean = (min + max) / 2f;
        float stdDev = (max - min) / 6f; // 99.7% of values within 3 std deviations

        float u1 = 1.0f - (float)random.NextDouble();
        float u2 = 1.0f - (float)random.NextDouble();
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        float randNormal = mean + stdDev * randStdNormal;

        return Mathf.Clamp(randNormal, min, max);
    }
}
