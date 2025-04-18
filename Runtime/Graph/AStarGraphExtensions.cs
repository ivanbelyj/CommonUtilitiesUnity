using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Based on https://github.com/PacktPublishing/Unity-5.x-Game-AI-Programming-Cookbook/blob/master/UAIPC/Assets/Scripts/Ch02Navigation/Graph.cs
public static class AStarExtensions
{
    public static List<TVertex> GetPathAstar<TVertex, TVertexValue>(
        this Graph<TVertex, TVertexValue> graph,
        Vector3 srcPos,
        Vector3 dstPos,
        PathCostHeuristic<TVertex, TVertexValue> h)
        where TVertex : Vertex<TVertexValue>
    {
        TVertex src = graph.GetNearestVertex(srcPos);
        TVertex dst = graph.GetNearestVertex(dstPos);

        // Debug.Log(
        //     $"Objects: {srcPos} and {dstPos}; "
        //     + $" Vertices: {src.gameObject.transform.position}"
        //     + $" and {dst.gameObject.transform.position}");

        var frontier = new GPWiki.BinaryHeap<Edge<TVertex, TVertexValue>>();
        
        Edge<TVertex, TVertexValue>[] edges;
        Edge<TVertex, TVertexValue> node, child;
        int size = graph.GetSize();
        float[] distValue = new float[size];
        int[] previous = new int[size];
        node = new Edge<TVertex, TVertexValue>(src, 0);
        frontier.Add(node);
        distValue[src.Id] = 0;
        previous[src.Id] = src.Id;
        for (int i = 0; i < size; i++)
        {
            if (i == src.Id)
                continue;
            distValue[i] = Mathf.Infinity;
            previous[i] = -1;
        }
        while (frontier.Count != 0)
        {
            node = frontier.Remove();
            int nodeId = node.Vertex.Id;
            if (ReferenceEquals(node.Vertex, dst))
            {    
                return graph.BuildPath(src.Id, node.Vertex.Id, ref previous);
            }
            edges = graph.GetEdges(node.Vertex);
            foreach (Edge<TVertex, TVertexValue> e in edges)
            {
                int eId = e.Vertex.Id;
                if (previous[eId] != -1)
                    continue;
                float cost = distValue[nodeId] + e.Cost;
                // key point
                cost += h(node.Vertex, e.Vertex);
                if (cost < distValue[e.Vertex.Id])
                {
                    distValue[eId] = cost;
                    previous[eId] = nodeId;
                    frontier.Remove(e);
                    child = new Edge<TVertex, TVertexValue>(e.Vertex, cost);
                    frontier.Add(child);
                }
            }
        }
        return new List<TVertex>();
    }

    private static List<TVertex> BuildPath<TVertex, TVertexValue>(
        this Graph<TVertex, TVertexValue> graph,
        int srcId,
        int dstId,
        ref int[] prevList)
        where TVertex : Vertex<TVertexValue>
    {
        var path = new LinkedList<TVertex>();
        int prev = dstId;
        do
        {
            path.AddFirst(graph.GetVertex(prev));
            prev = prevList[prev];
        } while (prev != srcId);
        return path.ToList();
    }
}
