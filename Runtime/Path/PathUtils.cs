using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathUtils
{
    public static bool IsPathClear(
        Vector2 origin,
        Vector2 destination,
        LayerMask obstacleLayerMask,
        float pathWidth)
    {
        Vector2 direction = destination - origin;
        float distance = direction.magnitude;
        direction.Normalize();

        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            origin,
            pathWidth / 2,
            direction,
            distance,
            obstacleLayerMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                return false;
            }
        }

        return true;
    }

    public static List<PositionedVertex<VertexEmptyValue>> SmoothPath(
        List<PositionedVertex<VertexEmptyValue>> path,
        LayerMask layerMask,
        float pathWidth = 0.5f)
    {
        var newPath = new List<PositionedVertex<VertexEmptyValue>>();
        if (path.Count <= 2)
            return path;

        newPath.Add(path[0]);

        int i, j;
        for (i = 0; i < path.Count - 1;)
        {
            for (j = i + 1; j < path.Count; j++)
            {
                Vector2 origin = path[i].Position;
                Vector2 destination = path[j].Position;

                if (!PathUtils.IsPathClear(origin, destination, layerMask, pathWidth))
                    break;
            }
            i = j - 1;
            newPath.Add(path[i]);
        }

        return newPath;
    }
}
