using System.Collections.Generic;
using UnityEngine;

public class Physics2DChecker : IPhysicsChecker
{
    private static Physics2DChecker instance;

    public static Physics2DChecker Instance
    {
        get
        {
            instance ??= new Physics2DChecker();
            return instance;
        }
    }

    private Physics2DChecker() { }

    public bool IsPathClear(Vector3 origin, Vector3 destination, LayerMask obstacleLayerMask, float pathWidth)
    {
        Vector2 direction = (Vector2)(destination - origin);
        float distance = direction.magnitude;

        if (distance < Mathf.Epsilon)
        {
            return true;
        }

        direction.Normalize();

        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            (Vector2)origin,
            pathWidth / 2f,
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

    public IEnumerable<GameObject> GetObstaclesBetween(Vector3 origin, Vector3 destination, LayerMask obstacleLayerMask, float pathWidth)
    {
        throw new System.NotImplementedException(
            $"TODO: Implement {nameof(GetObstaclesBetween)} for 2D");
    }

    public bool IsPathClearIgnoreObjects(Vector3 origin, Vector3 destination, LayerMask obstacleMask, float pathWidth, GameObject ignoreObject1, GameObject ignoreObject2)
    {
        throw new System.NotImplementedException(
            $"TODO: Implement {nameof(IsPathClearIgnoreObjects)} for 2D");
    }
}
