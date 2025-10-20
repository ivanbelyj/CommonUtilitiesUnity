using System.Collections.Generic;
using UnityEngine;

public class DynamicGraphAStarAdapter<TKey, TVertex, TValue> : IGraphForAStar<TVertex>
    where TVertex : DynamicVertex<TKey, TValue>
{
    private readonly DynamicGraph<TKey, TVertex, TValue> graph;

    public DynamicGraphAStarAdapter(DynamicGraph<TKey, TVertex, TValue> graph) => this.graph = graph;

    public TVertex GetNearestVertex(Vector3 position) => graph.GetNearestVertex(position);

    public IEnumerable<(TVertex neighbor, float cost)> GetNeighbors(TVertex v)
    {
        foreach (var link in v.Links)
        {
            yield return ((TVertex)link.TargetVertex, link.Cost);
        }
    }

    public bool AreEqual(TVertex a, TVertex b) => EqualityComparer<TKey>.Default.Equals(a.Key, b.Key);
}
