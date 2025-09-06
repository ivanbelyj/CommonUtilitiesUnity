using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Physics3DChecker : IPhysicsChecker
{
    private static Physics3DChecker instance;

    public static Physics3DChecker Instance
    {
        get
        {
            instance ??= new Physics3DChecker();
            return instance;
        }
    }

    private Physics3DChecker() { }

    public bool IsPathClear(
        Vector3 origin,
        Vector3 destination,
        LayerMask obstacleLayerMask,
        float pathWidth)
    {
        return GetRaycastHitsBetween(
            origin,
            destination,
            obstacleLayerMask,
            pathWidth).Length == 0;
    }

    public IEnumerable<GameObject> GetObstaclesBetween(
        Vector3 origin,
        Vector3 destination,
        LayerMask obstacleLayerMask,
        float pathWidth)
    {
        return GetRaycastHitsBetween(
            origin,
            destination,
            obstacleLayerMask,
            pathWidth)
            .Where(x => x.collider != null)
            .Select(x => x.collider.gameObject);
    }

    public bool IsPathClearIgnoreObjects(
        Vector3 origin,
        Vector3 destination,
        LayerMask obstacleMask,
        float pathWidth,
        GameObject ignoreObject1,
        GameObject ignoreObject2)
    {
        float radius = pathWidth / 2f;
        var hits = GetRaycastHitsBetween(origin, destination, obstacleMask, radius);

        foreach (var hit in hits)
        {
            var collider = hit.collider;
            if (collider != null && !collider.isTrigger)
            {
                if (!IsPartOfHierarchy(collider, ignoreObject1)
                    && !IsPartOfHierarchy(collider, ignoreObject2))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private RaycastHit[] GetRaycastHitsBetween(
        Vector3 origin,
        Vector3 destination,
        LayerMask obstacleLayerMask,
        float pathWidth)
    {
        Vector3 direction = destination - origin;
        float distance = direction.magnitude;

        direction.Normalize();

        return Physics.SphereCastAll(
            origin,
            pathWidth / 2f,
            direction,
            distance,
            obstacleLayerMask);
    }

    private bool IsPartOfHierarchy(Component component, GameObject target)
    {
        var current = component.transform;
        while (current != null)
        {
            if (current.gameObject == target)
            {
                return true;
            }
            current = current.parent;
        }
        return false;
    }
}
