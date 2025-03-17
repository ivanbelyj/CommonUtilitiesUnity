using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Allows to check for objects in the specified radius and provides according events
/// </summary>
public class RadiusScanner2D : IRadiusScanner
{
    public float Radius { get; set; }
    public int LayerMask { get; set; }
    public bool AllowDrawGizmos { get; set; }

    // Todo: remove commented ?
    // private GridManager gridManager;
    // private GridManager GridManager {
    //     get {
    //         // Works in OnDrawGizmos
    //         if (gridManager == null)
    //         {
    //             gridManager = GameObject
    //                 .Find("GridManager")
    //                 .GetComponent<GridManager>();
    //         }
    //         return gridManager;
    //     }
    // }

    public IEnumerable<DetectedObjectInfo> Scan(Vector3 originPosition, bool drawGizmos)
    {
        var objectsInRadius = GetObjectsWithDistanceInRadius(originPosition, drawGizmos);
        return objectsInRadius;
    }

    private IEnumerable<DetectedObjectInfo> GetObjectsWithDistanceInRadius(
        Vector3 originPosition,
        bool drawGizmos)
    {
        // Todo: Physics2D.OverlapCircleNonAlloc ?
        Collider2D[] overlappedByCircle = Physics2D.OverlapCircleAll(
            originPosition,
            Radius,
            LayerMask);
        var objectsInRadius = overlappedByCircle
            .Select(collider => {
                // Check is object in radius
                Vector3 objectPosition = collider.transform.position;
                float distance = GetDistance(originPosition, objectPosition);
                
                if (drawGizmos)
                {
                    HandleDrawGizmos(distance < Radius, objectPosition, originPosition);    
                }
                
                return new DetectedObjectInfo() {
                    distance = distance,
                    gameObject = collider.gameObject
                };
            });

        return objectsInRadius;
    }

    private float GetDistance(Vector3 v1, Vector3 v2) {
        return Vector3.Distance(v1, v2);

        // For some reason was implemented in Jailpunk previously.
        // Don't remember, why
        // return Vector3.Distance(ToCartesian(v1), ToCartesian(v2));

        // Vector3 ToCartesian(Vector3 vector) => GridManager.GridVectorToCartesian(vector);
    }

    private void HandleDrawGizmos(
        bool isDistanceLess,
        Vector3 objectPosition,
        Vector3 originPosition)
    {
        if (AllowDrawGizmos) {
            Color prevCol = Gizmos.color;
            Gizmos.color = isDistanceLess ? Color.green : Color.red;
            Gizmos.DrawLine(objectPosition, originPosition);
            Gizmos.color = prevCol;
        }
    }
}
