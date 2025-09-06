using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IEnumerableExtensions
{
    /// <summary>
    /// Checks if the collection is null or contains no elements
    /// </summary>
    public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
    {
        return source == null || !source.Any();
    }

    public static TSource MaxBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        if (keySelector == null)
        {
            throw new ArgumentNullException(nameof(keySelector));
        }

        var comparer = Comparer<TKey>.Default;

        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            throw new InvalidOperationException("Sequence contains no elements");
        }

        TSource maxElement = enumerator.Current;
        TKey maxKey = keySelector(maxElement);

        while (enumerator.MoveNext())
        {
            TSource current = enumerator.Current;
            TKey currentKey = keySelector(current);

            if (comparer.Compare(currentKey, maxKey) > 0)
            {
                maxElement = current;
                maxKey = currentKey;
            }
        }

        return maxElement;
    }

    /// <summary>
    /// Returns a sequence containing no duplicate elements according to the specified key selector.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <param name="source">The sequence to remove duplicates from.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>An IEnumerable<T> that contains distinct elements from the source sequence.</returns>
    /// <exception cref="ArgumentNullException">Thrown when source or keySelector is null.</exception>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (keySelector == null)
        {
            throw new ArgumentNullException(nameof(keySelector));
        }

        var seenKeys = new HashSet<TKey>();

        foreach (TSource element in source)
        {
            var key = keySelector(element);

            if (seenKeys.Add(key))
            {
                yield return element;
            }
        }
    }
}
