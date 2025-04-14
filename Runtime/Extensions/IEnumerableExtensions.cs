using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IEnumerableExtensions
{
    public static TSource MaxBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        return source.Aggregate(
            (maxSoFar, next) => Comparer<TKey>.Default.Compare(keySelector(next), keySelector(maxSoFar)) > 0 
                ? next 
                : maxSoFar);
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
