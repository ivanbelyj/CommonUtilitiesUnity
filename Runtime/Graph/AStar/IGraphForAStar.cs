using System.Collections.Generic;
using UnityEngine;

public interface IGraphForAStar<TVertex>
{
    TVertex GetNearestVertex(Vector3 position);
    IEnumerable<(TVertex neighbor, float cost)> GetNeighbors(TVertex vertex);
    bool AreEqual(TVertex a, TVertex b);
}
