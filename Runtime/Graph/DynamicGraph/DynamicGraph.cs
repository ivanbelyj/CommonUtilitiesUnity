using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DynamicVertex<TKey, TValue>
{
    private readonly List<DynamicEdge<TKey, TValue>> links = new();

    public TKey Key { get; }
    public TValue Value { get; set; }

    public IReadOnlyList<DynamicEdge<TKey, TValue>> Links => links;

    protected DynamicVertex(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public void EnsureLinkAdded(DynamicVertex<TKey, TValue> target, float cost)
    {
        if (Links.All(x => x.TargetVertex != target))
        {
            links.Add(new DynamicEdge<TKey, TValue>(target, cost));
        }
    }

    public int RemoveLinkByKey(TKey key)
    {
        return links.RemoveAll(x => x.TargetVertex.Key.Equals(key));
    }
}

public class DynamicEdge<TKey, TValue> : IComparable<DynamicEdge<TKey, TValue>>
{
    public DynamicVertex<TKey, TValue> TargetVertex { get; }
    public float Cost { get; }

    public DynamicEdge(DynamicVertex<TKey, TValue> targetVertex, float cost)
    {
        TargetVertex = targetVertex ?? throw new ArgumentNullException(nameof(targetVertex));
        Cost = cost;
    }

    public int CompareTo(DynamicEdge<TKey, TValue> other)
    {
        if (ReferenceEquals(TargetVertex, other.TargetVertex))
        {
            return 0;
        }

        var res = Cost - other.Cost;
        return (int)res;
    }
}

public abstract class DynamicGraph<TKey, TVertex, TValue>
    where TVertex : DynamicVertex<TKey, TValue>
{
    protected readonly Dictionary<TKey, TVertex> vertices = new();

    public IReadOnlyDictionary<TKey, TVertex> Vertices => vertices;

    public int Size => vertices.Count;

    public bool ContainsVertex(TKey key) => vertices.ContainsKey(key);

    public TVertex GetVertex(TKey key) => vertices.GetValueOrDefault(key);
    public bool TryGetVertex(TKey key, out TVertex value)
        => vertices.TryGetValue(key, out value);

    public void SetVertex(TKey key, TVertex vertex)
    {
        vertices[key] = vertex;
    }

    public virtual bool RemoveVertex(TKey key)
    {
        if (!vertices.TryGetValue(key, out var vertex))
        {
            return false;
        }

        foreach (var neighbour in vertex.Links)
        {
            neighbour.TargetVertex.RemoveLinkByKey(key);
        }

        vertices.Remove(key);
        return true;
    }

    public void EnsureLinkAdded(TKey fromKey, TKey toKey, float cost)
    {
        if (!vertices.TryGetValue(fromKey, out var from)
            || !vertices.TryGetValue(toKey, out var to))
        {
            throw new InvalidOperationException(
                $"Cannot create edge between vertices with keys '{fromKey}' and " +
                $"'{toKey}' - both vertices must be present in the graph.");
        }

        from.EnsureLinkAdded(to, cost);
    }

    public void RemoveEdge(TKey fromKey, TKey toKey)
    {
        if (vertices.TryGetValue(fromKey, out var from))
        {
            from.RemoveLinkByKey(toKey);
        }
        else
        {
            throw new InvalidOperationException(
                $"Cannot remove edge - vertex with key '{fromKey}' must present " +
                "in the graph.");
        }
    }

    public abstract TVertex GetNearestVertex(Vector3 position);
}