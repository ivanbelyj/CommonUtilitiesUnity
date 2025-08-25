using System.Collections.Generic;
using UnityEngine;

public static class PhysicsCheckerExtensions
{
    public static List<PositionedVertex<VertexEmptyValue>> SmoothPath(
        this IPhysicsChecker physicsChecker,
        List<PositionedVertex<VertexEmptyValue>> path,
        LayerMask layerMask,
        float pathWidth = 0.5f)
    {
        var newPath = new List<PositionedVertex<VertexEmptyValue>>();
        if (path.Count <= 2)
        {
            return path;
        }

        newPath.Add(path[0]);

        int i, j;
        for (i = 0; i < path.Count - 1;)
        {
            for (j = i + 1; j < path.Count; j++)
            {
                Vector2 origin = path[i].Position;
                Vector2 destination = path[j].Position;

                if (!physicsChecker.IsPathClear(origin, destination, layerMask, pathWidth))
                {
                    break;
                }
            }
            i = j - 1;
            newPath.Add(path[i]);
        }

        return newPath;
    }
}
