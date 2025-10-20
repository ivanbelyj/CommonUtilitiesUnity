using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public record VertexEmptyValue
{

}

[System.Serializable]
public class Vertex<TValue>
{
    public int Id { get; set; }
    public TValue Value { get; set; }
    public List<Edge<Vertex<TValue>, TValue>> Neighbours { get; set; }
    public Vertex<TValue> Prev { get; set; }
}
