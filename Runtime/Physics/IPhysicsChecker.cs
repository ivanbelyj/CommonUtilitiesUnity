using UnityEngine;

public interface IPhysicsChecker
{
    bool IsPathClear(
        Vector3 origin,
        Vector3 destination,
        LayerMask obstacleLayerMask,
        float pathWidth);
}
