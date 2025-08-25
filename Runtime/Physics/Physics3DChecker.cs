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
        Vector3 direction = destination - origin;
        float distance = direction.magnitude;

        if (distance < Mathf.Epsilon)
        {
            return true;
        }

        direction.Normalize();

        RaycastHit[] hits = Physics.SphereCastAll(
            origin,
            pathWidth / 2f,
            direction,
            distance,
            obstacleLayerMask);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                return false;
            }
        }
        return true;
    }
}
