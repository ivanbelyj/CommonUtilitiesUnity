// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public class DynamicGraph<TVertex, TVertexValue> : Graph<TVertex, TVertexValue>
//     where TVertex : PositionedVertex<TVertexValue>
// {
//     private Dictionary<int, TVertex> vertexMap;
//     private Dictionary<int, List<Edge<TVertex, TVertexValue>>> adjacencyList;

//     public DynamicGraph() : base(new List<TVertex>(), new List<List<TVertex>>(), new List<List<float>>())
//     {
//         vertexMap = new Dictionary<int, TVertex>();
//         adjacencyList = new Dictionary<int, List<Edge<TVertex, TVertexValue>>>();
//     }

//     public override TVertex GetVertex(int id)
//     {
//         vertexMap.TryGetValue(id, out var vertex);
//         return vertex;
//     }

//     public override TVertex[] GetNeighbours(int id)
//     {
//         if (adjacencyList.TryGetValue(id, out var edges))
//         {
//             var neighbours = new TVertex[edges.Count];
//             for (int i = 0; i < edges.Count; i++)
//             {
//                 neighbours[i] = edges[i].Vertex;
//             }
//             return neighbours;
//         }
//         return new TVertex[0];
//     }

//     public override Edge<TVertex, TVertexValue>[] GetEdges(TVertex v)
//     {
//         if (adjacencyList.TryGetValue(v.Id, out var edges))
//         {
//             return edges.ToArray();
//         }
//         return new Edge<TVertex, TVertexValue>[0];
//     }

//     public void AddVertex(TVertex vertex)
//     {
//         if (!vertexMap.ContainsKey(vertex.Id))
//         {
//             vertexMap[vertex.Id] = vertex;
//             adjacencyList[vertex.Id] = new List<Edge<TVertex, TVertexValue>>();
//         }
//     }

//     public void RemoveVertex(int id)
//     {
//         if (vertexMap.ContainsKey(id))
//         {
//             vertexMap.Remove(id);
//             adjacencyList.Remove(id);

//             foreach (var edges in adjacencyList.Values)
//             {
//                 edges.RemoveAll(edge => edge.Vertex.Id == id);
//             }
//         }
//     }

//     public void AddEdge(int fromId, int toId, float cost)
//     {
//         if (vertexMap.ContainsKey(fromId) && vertexMap.ContainsKey(toId))
//         {
//             var fromVertex = vertexMap[fromId];
//             var toVertex = vertexMap[toId];
//             adjacencyList[fromId].Add(new Edge<TVertex, TVertexValue>(toVertex, cost));
//         }
//     }

//     public void RemoveEdge(int fromId, int toId)
//     {
//         if (adjacencyList.ContainsKey(fromId))
//         {
//             adjacencyList[fromId].RemoveAll(edge => edge.Vertex.Id == toId);
//         }
//     }

//     // Todo: now not optimized
//     public override TVertex GetNearestVertex(Vector3 position)
//     {
//         TVertex nearestVertex = null;
//         float nearestDistance = float.MaxValue;

//         foreach (var vertex in vertexMap.Values)
//         {
//             float distance = Vector3.Distance(vertex.Position, position);
//             if (distance < nearestDistance)
//             {
//                 nearestDistance = distance;
//                 nearestVertex = vertex;
//             }
//         }

//         return nearestVertex;
//     }
// }
