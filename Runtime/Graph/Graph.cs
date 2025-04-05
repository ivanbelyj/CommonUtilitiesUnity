using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate float PathCostHeuristic<TVertex, TVertexValue>(TVertex a, TVertex b)
    where TVertex : Vertex<TVertexValue>;

public abstract class Graph<TVertex, TVertexValue> where TVertex : Vertex<TVertexValue>
{
    protected List<TVertex> Vertices { get; set; }
    protected List<List<TVertex>> Neighbours { get; set; }
    protected List<List<float>> Costs { get; set; }

    public Graph(
        List<TVertex> vertices,
        List<List<TVertex>> neighbours,
        List<List<float>> costs)
    {
        Vertices = vertices;
        Neighbours = neighbours;
        Costs = costs;
    }

    public IReadOnlyList<TVertex> GetVertices() => Vertices;

    public virtual int GetSize()
    {
        return Vertices?.Count ?? 0;
    }

    public abstract TVertex GetNearestVertex(Vector3 position);

    public virtual TVertex GetVertex(int id) {
        if (Vertices == null
            || Vertices.Count == 0
            || id < 0
            || id >= Vertices.Count)
            return null;
        
        return Vertices[id];
    }

    public virtual TVertex[] GetNeighbours(TVertex v) 
        => GetNeighbours(v.Id);

    public virtual TVertex[] GetNeighbours(int id) {
        if (Neighbours == null
            || Neighbours.Count == 0
            || id < 0
            || id >= Vertices.Count)
            return new TVertex[0];
        
        return Neighbours[id].ToArray();
    }

    public virtual Edge<TVertex, TVertexValue>[] GetEdges(TVertex v)
    {
        if (Neighbours == null || Neighbours.Count == 0)
            return new Edge<TVertex, TVertexValue>[0];
        if (v.Id < 0 || v.Id >= Neighbours.Count)
            return new Edge<TVertex, TVertexValue>[0];

        var numEdges = Neighbours[v.Id].Count;
        var edges = new Edge<TVertex, TVertexValue>[numEdges];
        var vertexList = Neighbours[v.Id];
        var costList = Costs[v.Id];
        for (int i = 0; i < numEdges; i++)
        {
            edges[i] = new Edge<TVertex, TVertexValue>(vertexList[i], costList[i]);
        }
        
        return edges;
    }
}
