using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathCostHeuristicUtils
{
    public static float ManhattanEstimate<TVertexValue>(
        PositionedVertex<TVertexValue> a,
        PositionedVertex<TVertexValue> b)
    {
        Vector3 posA = a.Position;
        Vector3 posB = b.Position;
        return Math.Abs(posA.x - posB.x) + Math.Abs(posA.y - posB.y);
    }

    public static float EuclideanEstimate<TVertexValue>(
        PositionedVertex<TVertexValue> a,
        PositionedVertex<TVertexValue> b)
    {
        Vector3 posA = a.Position;
        Vector3 posB = b.Position;
        return Vector3.Distance(posA, posB);
    }
}
