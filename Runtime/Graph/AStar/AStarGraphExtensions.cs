// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// // Based on https://github.com/PacktPublishing/Unity-5.x-Game-AI-Programming-Cookbook/blob/master/UAIPC/Assets/Scripts/Ch02Navigation/Graph.cs
// public static class AStarExtensions
// {
//     public static List<TVertex> GetPathAstar<TVertex, TVertexValue>(
//         this Graph<TVertex, TVertexValue> graph,
//         Vector3 srcPos,
//         Vector3 dstPos,
//         PathCostHeuristic<TVertex, TVertexValue> h)
//         where TVertex : Vertex<TVertexValue>
//     {
//         TVertex src = graph.GetNearestVertex(srcPos);
//         TVertex dst = graph.GetNearestVertex(dstPos);

//         // Debug.Log(
//         //     $"Objects: {srcPos} and {dstPos}; "
//         //     + $" Vertices: {src.gameObject.transform.position}"
//         //     + $" and {dst.gameObject.transform.position}");

//         var frontier = new GPWiki.BinaryHeap<Edge<TVertex, TVertexValue>>();

//         Edge<TVertex, TVertexValue>[] edges;
//         Edge<TVertex, TVertexValue> node, child;
//         int size = graph.GetSize();
//         float[] distValue = new float[size];
//         int[] previous = new int[size];
//         node = new Edge<TVertex, TVertexValue>(src, 0);
//         frontier.Add(node);
//         distValue[src.Id] = 0;
//         previous[src.Id] = src.Id;
//         for (int i = 0; i < size; i++)
//         {
//             if (i == src.Id)
//                 continue;
//             distValue[i] = Mathf.Infinity;
//             previous[i] = -1;
//         }
//         while (frontier.Count != 0)
//         {
//             node = frontier.Remove();
//             int nodeId = node.Vertex.Id;
//             if (ReferenceEquals(node.Vertex, dst))
//             {    
//                 return graph.BuildPath(src.Id, node.Vertex.Id, ref previous);
//             }
//             edges = graph.GetEdges(node.Vertex);
//             foreach (Edge<TVertex, TVertexValue> e in edges)
//             {
//                 int eId = e.Vertex.Id;
//                 if (previous[eId] != -1)
//                     continue;
//                 float cost = distValue[nodeId] + e.Cost;
//                 // key point
//                 cost += h(node.Vertex, e.Vertex);
//                 if (cost < distValue[e.Vertex.Id])
//                 {
//                     distValue[eId] = cost;
//                     previous[eId] = nodeId;
//                     frontier.Remove(e);
//                     child = new Edge<TVertex, TVertexValue>(e.Vertex, cost);
//                     frontier.Add(child);
//                 }
//             }
//         }
//         return new List<TVertex>();
//     }

//     private static List<TVertex> BuildPath<TVertex, TVertexValue>(
//         this Graph<TVertex, TVertexValue> graph,
//         int srcId,
//         int dstId,
//         ref int[] prevList)
//         where TVertex : Vertex<TVertexValue>
//     {
//         var path = new LinkedList<TVertex>();
//         int prev = dstId;
//         do
//         {
//             path.AddFirst(graph.GetVertex(prev));
//             prev = prevList[prev];
//         } while (prev != srcId);
//         return path.ToList();
//     }
// }

// Version optimized by DeepSeek (~x10 performance comparing with previous)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        if (src == null || dst == null || ReferenceEquals(src, dst))
            return new List<TVertex>();

        int size = graph.GetSize();
        if (size == 0) return new List<TVertex>();

        // Используем существующий конструктор BinaryHeap
        var openSet = new GPWiki.BinaryHeap<Edge<TVertex, TVertexValue>>();

        // Оптимизированные структуры данных
        var gScore = new float[size];    // Стоимость от начала
        var fScore = new float[size];    // g + h (приоритет)
        var cameFrom = new int[size];    // Предыдущие узлы
        var inOpenSet = new bool[size];  // Отслеживание в открытом множестве
        var inClosedSet = new bool[size]; // Закрытое множество

        // Инициализация
        for (int i = 0; i < size; i++)
        {
            gScore[i] = Mathf.Infinity;
            fScore[i] = Mathf.Infinity;
            cameFrom[i] = -1;
        }

        gScore[src.Id] = 0;
        fScore[src.Id] = h(src, dst);
        openSet.Add(new Edge<TVertex, TVertexValue>(src, fScore[src.Id]));
        inOpenSet[src.Id] = true;
        cameFrom[src.Id] = src.Id;

        while (openSet.Count > 0)
        {
            // Берем узел с наименьшей fScore
            var currentEdge = openSet.Remove();
            int currentId = currentEdge.Vertex.Id;

            if (ReferenceEquals(currentEdge.Vertex, dst))
            {
                return graph.BuildPath(src.Id, currentId, ref cameFrom);
            }

            inOpenSet[currentId] = false;
            inClosedSet[currentId] = true;

            // Обрабатываем соседей
            var edges = graph.GetEdges(currentEdge.Vertex);
            foreach (var edge in edges)
            {
                int neighborId = edge.Vertex.Id;

                // Пропускаем уже обработанные узлы
                if (inClosedSet[neighborId])
                    continue;

                // Вычисляем временную стоимость
                float tentativeGScore = gScore[currentId] + edge.Cost;

                if (tentativeGScore < gScore[neighborId])
                {
                    cameFrom[neighborId] = currentId;
                    gScore[neighborId] = tentativeGScore;
                    float newFScore = tentativeGScore + h(edge.Vertex, dst);
                    fScore[neighborId] = newFScore;

                    if (inOpenSet[neighborId])
                    {
                        // Вместо удаления/добавления, просто добавляем новый элемент с лучшим приоритетом
                        // Старый элемент останется в куче с худшим приоритетом, но мы его проигнорируем
                        openSet.Add(new Edge<TVertex, TVertexValue>(edge.Vertex, newFScore));
                    }
                    else
                    {
                        openSet.Add(new Edge<TVertex, TVertexValue>(edge.Vertex, newFScore));
                        inOpenSet[neighborId] = true;
                    }
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
        int current = dstId;

        while (current != srcId)
        {
            path.AddFirst(graph.GetVertex(current));
            current = prevList[current];
        }
        path.AddFirst(graph.GetVertex(srcId));

        return path.ToList();
    }

    #region More flexible implementation
    // Todo: Review and test properly.
    // It seems to work, but it's not tested and not reviewed properly.

    private struct AStarNode<TVertex> : IComparable<AStarNode<TVertex>>
    {
        public TVertex Vertex { get; }
        public float FScore { get; }

        public AStarNode(TVertex vertex, float fScore)
        {
            Vertex = vertex;
            FScore = fScore;
        }

        public int CompareTo(AStarNode<TVertex> other)
        {
            return FScore.CompareTo(other.FScore);
        }
    }

    public static List<TVertex> GetPathAStar<TVertex>(
        this IGraphForAStar<TVertex> graph,
        Vector3 srcPos,
        Vector3 dstPos,
        Func<TVertex, TVertex, float> heuristic)
    {
        var src = graph.GetNearestVertex(srcPos);
        var dst = graph.GetNearestVertex(dstPos);

        if (src == null || dst == null || graph.AreEqual(src, dst))
            return new List<TVertex>();

        var openSet = new GPWiki.BinaryHeap<AStarNode<TVertex>>();
        var gScore = new Dictionary<TVertex, float>();
        var fScore = new Dictionary<TVertex, float>();
        var cameFrom = new Dictionary<TVertex, TVertex>();
        var closedSet = new HashSet<TVertex>();

        gScore[src] = 0;
        fScore[src] = heuristic(src, dst);
        openSet.Add(new AStarNode<TVertex>(src, fScore[src]));

        while (openSet.Count > 0)
        {
            var currentNode = openSet.Remove();
            var current = currentNode.Vertex;

            if (graph.AreEqual(current, dst))
            {
                return ReconstructPath(cameFrom, current);
            }

            closedSet.Add(current);

            foreach (var (neighbor, cost) in graph.GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeG = gScore.GetValueOrDefault(current, float.MaxValue) + cost;
                if (tentativeG < gScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + heuristic(neighbor, dst);
                    openSet.Add(new AStarNode<TVertex>(neighbor, fScore[neighbor]));
                }
            }
        }

        return new List<TVertex>();
    }

    private static List<TVertex> ReconstructPath<TVertex>(
        Dictionary<TVertex, TVertex> cameFrom,
        TVertex current)
    {
        var path = new List<TVertex> { current };
        while (cameFrom.TryGetValue(current, out var prev))
        {
            path.Add(prev);
            current = prev;
        }
        path.Reverse();
        return path;
    }

    #endregion
}
