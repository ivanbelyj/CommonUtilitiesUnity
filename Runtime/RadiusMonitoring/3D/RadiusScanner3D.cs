// Todo: Remove the commented ai-generated code if the next version is OK.

// using System.Collections.Generic;
// using UnityEngine;

// public class RadiusScanner3D : IRadiusScanner
// {
//     public float Radius { get; set; }
//     public int LayerMask { get; set; }
//     public bool AllowDrawGizmos { get; set; }

//     private Collider[] overlapBuffer;
//     private DetectedObjectInfo[] resultsArray;
//     private const int InitialBufferSize = 32;
//     private const int WarningBufferSize = 1024;
//     private const int CriticalBufferSize = 5120;
//     private const int MaxBufferSize = 8192;

//     private bool warningIssued = false;
//     private string lastWarning = "";

//     public RadiusScanner3D()
//     {
//         overlapBuffer = new Collider[InitialBufferSize];
//         resultsArray = new DetectedObjectInfo[InitialBufferSize];
//     }

//     public IEnumerable<DetectedObjectInfo> Scan(Vector3 originPosition, bool drawGizmos)
//     {
//         int detectedCount = Physics.OverlapSphereNonAlloc(
//             originPosition,
//             Radius,
//             overlapBuffer,
//             LayerMask
//         );

//         if (detectedCount == overlapBuffer.Length)
//         {
//             if (!HandleBufferOverflow(ref detectedCount, originPosition))
//             {
//                 return System.Array.Empty<DetectedObjectInfo>();
//             }
//         }

//         return ProcessDetectedObjects(detectedCount, originPosition, drawGizmos);
//     }

//     private bool HandleBufferOverflow(ref int detectedCount, Vector3 originPosition)
//     {
//         if (detectedCount >= CriticalBufferSize)
//         {
//             LogCriticalError(detectedCount);
//             return false;
//         }

//         if (detectedCount < MaxBufferSize)
//         {
//             int newSize = Mathf.Min(Mathf.NextPowerOfTwo(detectedCount * 2), MaxBufferSize);
//             LogWarningIfNeeded(newSize);

//             overlapBuffer = new Collider[newSize];
//             resultsArray = new DetectedObjectInfo[newSize];

//             detectedCount = Physics.OverlapSphereNonAlloc(
//                 originPosition,
//                 Radius,
//                 overlapBuffer,
//                 LayerMask
//             );

//             if (detectedCount == overlapBuffer.Length && detectedCount >= CriticalBufferSize)
//             {
//                 LogCriticalError(detectedCount);
//                 return false;
//             }
//         }
//         else
//         {
//             Debug.LogError($"RadiusScanner reached maximum buffer size: {MaxBufferSize}.");
//             return false;
//         }

//         return true;
//     }

//     private IEnumerable<DetectedObjectInfo> ProcessDetectedObjects(int detectedCount, Vector3 originPosition, bool drawGizmos)
//     {
//         float radiusSqr = Radius * Radius;
//         int validCount = 0;

//         for (int i = 0; i < detectedCount; i++)
//         {
//             Collider collider = overlapBuffer[i];
//             if (collider == null) continue;

//             Vector3 objectPosition = collider.transform.position;
//             Vector3 direction = objectPosition - originPosition;
//             float distanceSqr = direction.sqrMagnitude;

//             bool isInRadius = distanceSqr <= radiusSqr;

//             if (drawGizmos)
//             {
//                 HandleDrawGizmos(isInRadius, objectPosition, originPosition);
//             }

//             if (isInRadius)
//             {
//                 resultsArray[validCount] = new DetectedObjectInfo
//                 {
//                     gameObject = collider.gameObject,
//                     distance = Mathf.Sqrt(distanceSqr)
//                 };
//                 validCount++;
//             }
//         }

//         return GetResultsEnumerable(validCount);
//     }

//     private IEnumerable<DetectedObjectInfo> GetResultsEnumerable(int count)
//     {
//         for (int i = 0; i < count; i++)
//         {
//             yield return resultsArray[i];
//         }
//     }

//     private void HandleDrawGizmos(bool isDistanceLess, Vector3 objectPosition, Vector3 originPosition)
//     {
//         if (AllowDrawGizmos)
//         {
//             Gizmos.color = isDistanceLess ? Color.green : Color.red;
//             Gizmos.DrawLine(objectPosition, originPosition);
//         }
//     }

//     private void LogWarningIfNeeded(int newSize)
//     {
//         if (newSize >= WarningBufferSize && !warningIssued)
//         {
//             warningIssued = true;
//             lastWarning = $"WARNING: RadiusScanner buffer resized to {newSize}. Radius: {Radius}";
//             Debug.LogWarning(lastWarning);
//         }
//     }

//     private void LogCriticalError(int detectedCount)
//     {
//         string errorMessage =
//             $"CRITICAL: RadiusScanner buffer overflow! Required: {detectedCount}, " +
//             $"Max: {MaxBufferSize}. Reduce radius or optimize colliders.";
//         Debug.LogError(errorMessage);
//     }

//     public void EnsureBufferCapacity(int requiredCapacity)
//     {
//         if (requiredCapacity > MaxBufferSize)
//         {
//             Debug.LogError($"Cannot ensure capacity: {requiredCapacity} > {MaxBufferSize}");
//             return;
//         }

//         if (overlapBuffer.Length < requiredCapacity)
//         {
//             int newSize = Mathf.NextPowerOfTwo(requiredCapacity);
//             if (newSize >= WarningBufferSize)
//             {
//                 Debug.LogWarning($"Manual buffer resize to: {newSize}");
//             }

//             overlapBuffer = new Collider[newSize];
//             resultsArray = new DetectedObjectInfo[newSize];
//         }
//     }

//     public string GetLastWarning() => lastWarning;
//     public void ClearWarnings() => warningIssued = false;
//     public int GetCurrentBufferSize() => overlapBuffer.Length;
//     public void Dispose() => ClearWarnings();
// }

using System.Collections.Generic;
using UnityEngine;

// Todo: Review ai-generated code.

/// <summary>
/// 3D radius scanner. Uses Physics.OverlapSphereNonAlloc with a reusable buffer
/// to avoid per-frame allocations. Grows the buffer once on overflow and rescans.
/// </summary>
public class RadiusScanner3D : IRadiusScanner
{
    private const int InitialBufferSize = 32;
    private const int WarningBufferSize = 1024;
    private const int HardCapBufferSize = 8192;

    // Reused across scans — no allocation in the hot path once grown.
    private Collider[] overlapBuffer;

    // Reused results list. Caller must not cache it across frames.
    private readonly List<DetectedObjectInfo> results;

    private bool bufferWarningIssued;

    public float Radius { get; set; }
    public int LayerMask { get; set; }
    public bool AllowDrawGizmos { get; set; }

    public RadiusScanner3D()
    {
        overlapBuffer = new Collider[InitialBufferSize];
        results = new List<DetectedObjectInfo>(InitialBufferSize);
    }

    public IReadOnlyList<DetectedObjectInfo> Scan(Vector3 originPosition, bool drawGizmos)
    {
        results.Clear();

        int detectedCount = Physics.OverlapSphereNonAlloc(
            originPosition,
            Radius,
            overlapBuffer,
            LayerMask,
            QueryTriggerInteraction.UseGlobal);

        // NonAlloc silently truncates when the buffer is exactly full.
        // Grow the buffer and rescan once so we don't miss any colliders.
        if (detectedCount == overlapBuffer.Length)
        {
            if (!GrowBufferAndRescan(originPosition, ref detectedCount))
            {
                // Buffer cap reached; return whatever we managed to collect.
                return results;
            }
        }

        // Compare squared distances to avoid sqrt per candidate.
        float radiusSqr = Radius * Radius;
        bool draw = drawGizmos && AllowDrawGizmos;

        for (int i = 0; i < detectedCount; i++)
        {
            Collider collider = overlapBuffer[i];
            if (collider == null)
            {
                // Collider may have been destroyed between frames.
                continue;
            }

            Vector3 objectPosition = collider.transform.position;
            Vector3 direction = objectPosition - originPosition;
            float distanceSqr = direction.sqrMagnitude;
            bool isInRadius = distanceSqr <= radiusSqr;

            if (draw)
            {
                DrawGizmo(isInRadius, objectPosition, originPosition);
            }

            if (isInRadius)
            {
                results.Add(new DetectedObjectInfo
                {
                    gameObject = collider.gameObject,
                    distance = Mathf.Sqrt(distanceSqr)
                });
            }
        }

        return results;
    }

    /// <summary>
    /// Doubles the buffer size and rescans. Returns false if the hard cap is reached
    /// or the buffer is still full after growing (meaning we cannot be sure we got all colliders).
    /// </summary>
    private bool GrowBufferAndRescan(Vector3 originPosition, ref int detectedCount)
    {
        int currentSize = overlapBuffer.Length;
        int newSize = currentSize * 2;

        if (newSize > HardCapBufferSize)
        {
            Debug.LogError(
                $"RadiusScanner3D buffer overflow: hard cap of {HardCapBufferSize} reached. " +
                $"Radius: {Radius}. Reduce radius or optimize colliders.");
            return false;
        }

        if (newSize >= WarningBufferSize && !bufferWarningIssued)
        {
            bufferWarningIssued = true;
            Debug.LogWarning(
                $"RadiusScanner3D buffer grew to {newSize}. Radius: {Radius}. " +
                $"Consider reducing radius or optimizing colliders.");
        }

        overlapBuffer = new Collider[newSize];

        detectedCount = Physics.OverlapSphereNonAlloc(
            originPosition,
            Radius,
            overlapBuffer,
            LayerMask,
            QueryTriggerInteraction.UseGlobal);

        // Still full after doubling? We can't guarantee completeness.
        if (detectedCount == overlapBuffer.Length)
        {
            Debug.LogError(
                $"RadiusScanner3D buffer is still full after resize to {newSize}. " +
                $"Some colliders will be missing from the result.");
            return false;
        }

        return true;
    }

    private static void DrawGizmo(bool isInRadius, Vector3 objectPosition, Vector3 originPosition)
    {
        Color prevCol = Gizmos.color;
        Gizmos.color = isInRadius ? Color.green : Color.red;
        Gizmos.DrawLine(objectPosition, originPosition);
        Gizmos.color = prevCol;
    }

    public void ClearWarnings() => bufferWarningIssued = false;
    public int GetCurrentBufferSize() => overlapBuffer.Length;
}
