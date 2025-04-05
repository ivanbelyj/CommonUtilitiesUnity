using UnityEngine;

public class PositionedVertex<TValue> : Vertex<TValue>
{
    public Vector3 Position { get; set; }
    
    public PositionedVertex(Vector3 position)
    {
        Position = position;
    }
}
