using System.Collections.Generic;
using UnityEngine;

public interface IPhysicsChecker
{
    bool IsPathClear(
        Vector3 origin,
        Vector3 destination,
        LayerMask obstacleLayerMask,
        float pathWidth);

    IEnumerable<GameObject> GetObstaclesBetween(
        Vector3 origin,
        Vector3 destination,
        LayerMask obstacleLayerMask,
        float pathWidth);

    bool IsPathClearIgnoreObjects(
        Vector3 origin,
        Vector3 destination,
        LayerMask obstacleMask,
        float pathWidth,
        GameObject ignoreObject1,
        GameObject ignoreObject2);
}
