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

public static class RandomUtils
{
    /// <summary>
    /// Selects a random element from the list with equal probability for each item.
    /// </summary>
    /// <returns>A randomly selected item.</returns>
    public static T GetRandomOne<T>(IList<T> items, System.Random random)
        => items[(int)((random == null ? Random.value : random.NextDoubleInclusive()) * items.Count)];

    /// <summary>
    /// Selects a random element from the list using weighted probabilities (roulette wheel algorithm).
    /// </summary>
    /// <param name="items">The list of items to choose from.</param>
    /// <param name="weights">The list of weights corresponding to each item.</param>
    /// <returns>A randomly selected item based on the provided weights.</returns>
    public static T GetRandomWeighted<T>(IList<T> items, IList<float> weights, System.Random random = null)
    {
        if (items == null || weights == null
            || items.Count != weights.Count || items.Count == 0)
        {
            throw new System.ArgumentException("Items and weights must be non-null, of the same length, and non-empty.");
        }

        float totalWeight = 0f;
        foreach (float weight in weights)
            totalWeight += weight;

        float randomValue = (random == null ? Random.value : (float)random.NextDoubleInclusive())
            * totalWeight;
        float cumulativeWeight = 0f;

        for (int i = 0; i < items.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
                return items[i];
        }

        return items[items.Count - 1];
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
