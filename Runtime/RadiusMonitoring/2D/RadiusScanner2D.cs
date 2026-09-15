// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// /// <summary>
// /// Allows to check for objects in the specified radius and provides according events
// /// </summary>
// public class RadiusScanner2D : IRadiusScanner
// {
//     public float Radius { get; set; }
//     public int LayerMask { get; set; }
//     public bool AllowDrawGizmos { get; set; }

//     public IEnumerable<DetectedObjectInfo> Scan(Vector3 originPosition, bool drawGizmos)
//     {
//         var objectsInRadius = GetObjectsWithDistanceInRadius(originPosition, drawGizmos);
//         return objectsInRadius;
//     }

//     private IEnumerable<DetectedObjectInfo> GetObjectsWithDistanceInRadius(
//         Vector3 originPosition,
//         bool drawGizmos)
//     {
//         // Todo: Physics2D.OverlapCircleNonAlloc ?
//         Collider2D[] overlappedByCircle = Physics2D.OverlapCircleAll(
//             originPosition,
//             Radius,
//             LayerMask);
//         var objectsInRadius = overlappedByCircle
//             .Select(collider =>
//             {
//                 // Check is object in radius
//                 Vector3 objectPosition = collider.transform.position;
//                 float distance = GetDistance(originPosition, objectPosition);

//                 if (drawGizmos)
//                 {
//                     HandleDrawGizmos(distance < Radius, objectPosition, originPosition);
//                 }

//                 return new DetectedObjectInfo()
//                 {
//                     distance = distance,
//                     gameObject = collider.gameObject
//                 };
//             });

//         return objectsInRadius;
//     }

//     private float GetDistance(Vector3 v1, Vector3 v2)
//     {
//         return Vector3.Distance(v1, v2);

//         // For some reason was implemented in Jailpunk previously.
//         // Don't remember, why
//         // return Vector3.Distance(ToCartesian(v1), ToCartesian(v2));

//         // Vector3 ToCartesian(Vector3 vector) => GridManager.GridVectorToCartesian(vector);
//     }

//     private void HandleDrawGizmos(
//         bool isDistanceLess,
//         Vector3 objectPosition,
//         Vector3 originPosition)
//     {
//         if (AllowDrawGizmos)
//         {
//             Color prevCol = Gizmos.color;
//             Gizmos.color = isDistanceLess ? Color.green : Color.red;
//             Gizmos.DrawLine(objectPosition, originPosition);
//             Gizmos.color = prevCol;
//         }
//     }
// }

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows to check for objects in the specified radius and provides according events.
/// Zero allocations per Scan() call when the internal buffers are large enough.
/// </summary>
public class RadiusScanner2D : IRadiusScanner
{
    private const int InitialCapacity = 32;

    // Reused across calls — no per-frame allocation.
    private readonly List<Collider2D> colliderBuffer = new List<Collider2D>(InitialCapacity);
    private readonly List<DetectedObjectInfo> results = new List<DetectedObjectInfo>(InitialCapacity);

    // Reused ContactFilter2D — avoid allocating it per call.
    private ContactFilter2D contactFilter;
    private bool isContactFilterInitialized;

    public float Radius { get; set; }
    public int LayerMask { get; set; }
    public bool AllowDrawGizmos { get; set; }

    public IReadOnlyList<DetectedObjectInfo> Scan(Vector3 originPosition, bool drawGizmos)
    {
        results.Clear();
        colliderBuffer.Clear();

        EnsureContactFilter();

        // The List-based overload grows the list itself; no truncation.
        Physics2D.OverlapCircle(originPosition, Radius, contactFilter, colliderBuffer);

        int count = colliderBuffer.Count;
        for (int i = 0; i < count; i++)
        {
            Collider2D collider = colliderBuffer[i];
            if (collider == null)
            {
                continue;
            }

            Vector3 objectPosition = collider.transform.position;
            float distance = Vector3.Distance(originPosition, objectPosition);

            if (drawGizmos && AllowDrawGizmos)
            {
                DrawGizmo(distance < Radius, objectPosition, originPosition);
            }

            results.Add(new DetectedObjectInfo
            {
                distance = distance,
                gameObject = collider.gameObject
            });
        }

        return results;
    }

    private void EnsureContactFilter()
    {
        // LayerMask is mutable, so we re-sync it on each scan (cheap).
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = LayerMask;

        contactFilter.useTriggers = true;

        if (!isContactFilterInitialized)
        {
            contactFilter.useDepth = false;
            contactFilter.useOutsideDepth = false;
            contactFilter.useNormalAngle = false;
            contactFilter.useOutsideNormalAngle = false;
            isContactFilterInitialized = true;
        }
    }

    private static void DrawGizmo(bool isDistanceLess, Vector3 objectPosition, Vector3 originPosition)
    {
        Color prevCol = Gizmos.color;
        Gizmos.color = isDistanceLess ? Color.green : Color.red;
        Gizmos.DrawLine(objectPosition, originPosition);
        Gizmos.color = prevCol;
    }
}
