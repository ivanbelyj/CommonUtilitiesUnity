using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge<TVertex, TValue> : IComparable<Edge<TVertex, TValue>>
    where TVertex : Vertex<TValue>
{
    public TVertex Vertex { get; set; }
    public float Cost { get; set; }

    public Edge(TVertex vertex, float cost)
    {
        Vertex = vertex;
        Cost = cost;
    }

    public int CompareTo(Edge<TVertex, TValue> other)
    {
        if (ReferenceEquals(Vertex, other.Vertex))
            return 0;
        
        float res = Cost - other.Cost;
        return (int)res;
    }

    public bool Equals(Edge<TVertex, TValue> other) {
        return Vertex.Id == other.Vertex.Id;
    }

    public override bool Equals(object other) {
        if (other is Edge<TVertex, TValue>) 
            return Equals(other);

        return false;
    }
    public override int GetHashCode()
    {
        return Vertex.GetHashCode();
    }
}
