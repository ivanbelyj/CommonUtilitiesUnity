using System.Collections.Generic;
using UnityEngine;

public class GraphAStarAdapter<TVertex, TValue> : IGraphForAStar<TVertex>
    where TVertex : Vertex<TValue>
{
    private readonly Graph<TVertex, TValue> graph;

    public GraphAStarAdapter(Graph<TVertex, TValue> graph) => this.graph = graph;

    public TVertex GetNearestVertex(Vector3 position) => graph.GetNearestVertex(position);

    public IEnumerable<(TVertex, float)> GetNeighbors(TVertex v)
    {
        var edges = graph.GetEdges(v);
        foreach (var edge in edges)
        {
            yield return (edge.Vertex, edge.Cost);
        }
    }

    public bool AreEqual(TVertex a, TVertex b) => ReferenceEquals(a, b);
}
