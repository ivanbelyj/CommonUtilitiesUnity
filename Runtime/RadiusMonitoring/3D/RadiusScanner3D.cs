using System.Collections.Generic;
using UnityEngine;

// AI-Generated
// Todo: review

public class RadiusScanner3D : IRadiusScanner
{
    public float Radius { get; set; }
    public int LayerMask { get; set; }
    public bool AllowDrawGizmos { get; set; }

    private Collider[] overlapBuffer;
    private DetectedObjectInfo[] resultsArray;
    private const int InitialBufferSize = 32;
    private const int WarningBufferSize = 1024;
    private const int CriticalBufferSize = 5120;
    private const int MaxBufferSize = 8192;

    private bool warningIssued = false;
    private string lastWarning = "";

    public RadiusScanner3D()
    {
        overlapBuffer = new Collider[InitialBufferSize];
        resultsArray = new DetectedObjectInfo[InitialBufferSize];
    }

    public IEnumerable<DetectedObjectInfo> Scan(Vector3 originPosition, bool drawGizmos)
    {
        int detectedCount = Physics.OverlapSphereNonAlloc(
            originPosition,
            Radius,
            overlapBuffer,
            LayerMask
        );

        if (detectedCount == overlapBuffer.Length)
        {
            if (!HandleBufferOverflow(ref detectedCount, originPosition))
            {
                return System.Array.Empty<DetectedObjectInfo>();
            }
        }

        return ProcessDetectedObjects(detectedCount, originPosition, drawGizmos);
    }

    private bool HandleBufferOverflow(ref int detectedCount, Vector3 originPosition)
    {
        if (detectedCount >= CriticalBufferSize)
        {
            LogCriticalError(detectedCount);
            return false;
        }

        if (detectedCount < MaxBufferSize)
        {
            int newSize = Mathf.Min(Mathf.NextPowerOfTwo(detectedCount * 2), MaxBufferSize);
            LogWarningIfNeeded(newSize);

            overlapBuffer = new Collider[newSize];
            resultsArray = new DetectedObjectInfo[newSize];

            detectedCount = Physics.OverlapSphereNonAlloc(
                originPosition,
                Radius,
                overlapBuffer,
                LayerMask
            );

            if (detectedCount == overlapBuffer.Length && detectedCount >= CriticalBufferSize)
            {
                LogCriticalError(detectedCount);
                return false;
            }
        }
        else
        {
            Debug.LogError($"RadiusScanner reached maximum buffer size: {MaxBufferSize}.");
            return false;
        }

        return true;
    }

    private IEnumerable<DetectedObjectInfo> ProcessDetectedObjects(int detectedCount, Vector3 originPosition, bool drawGizmos)
    {
        float radiusSqr = Radius * Radius;
        int validCount = 0;

        for (int i = 0; i < detectedCount; i++)
        {
            Collider collider = overlapBuffer[i];
            if (collider == null) continue;

            Vector3 objectPosition = collider.transform.position;
            Vector3 direction = objectPosition - originPosition;
            float distanceSqr = direction.sqrMagnitude;

            bool isInRadius = distanceSqr <= radiusSqr;

            if (drawGizmos)
            {
                HandleDrawGizmos(isInRadius, objectPosition, originPosition);
            }

            if (isInRadius)
            {
                resultsArray[validCount] = new DetectedObjectInfo
                {
                    gameObject = collider.gameObject,
                    distance = Mathf.Sqrt(distanceSqr)
                };
                validCount++;
            }
        }

        return GetResultsEnumerable(validCount);
    }

    private IEnumerable<DetectedObjectInfo> GetResultsEnumerable(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return resultsArray[i];
        }
    }

    private void HandleDrawGizmos(bool isDistanceLess, Vector3 objectPosition, Vector3 originPosition)
    {
        if (AllowDrawGizmos)
        {
            Gizmos.color = isDistanceLess ? Color.green : Color.red;
            Gizmos.DrawLine(objectPosition, originPosition);
        }
    }

    private void LogWarningIfNeeded(int newSize)
    {
        if (newSize >= WarningBufferSize && !warningIssued)
        {
            warningIssued = true;
            lastWarning = $"WARNING: RadiusScanner buffer resized to {newSize}. Radius: {Radius}";
            Debug.LogWarning(lastWarning);
        }
    }

    private void LogCriticalError(int detectedCount)
    {
        string errorMessage =
            $"CRITICAL: RadiusScanner buffer overflow! Required: {detectedCount}, " +
            $"Max: {MaxBufferSize}. Reduce radius or optimize colliders.";
        Debug.LogError(errorMessage);
    }

    public void EnsureBufferCapacity(int requiredCapacity)
    {
        if (requiredCapacity > MaxBufferSize)
        {
            Debug.LogError($"Cannot ensure capacity: {requiredCapacity} > {MaxBufferSize}");
            return;
        }

        if (overlapBuffer.Length < requiredCapacity)
        {
            int newSize = Mathf.NextPowerOfTwo(requiredCapacity);
            if (newSize >= WarningBufferSize)
            {
                Debug.LogWarning($"Manual buffer resize to: {newSize}");
            }

            overlapBuffer = new Collider[newSize];
            resultsArray = new DetectedObjectInfo[newSize];
        }
    }

    public string GetLastWarning() => lastWarning;
    public void ClearWarnings() => warningIssued = false;
    public int GetCurrentBufferSize() => overlapBuffer.Length;
    public void Dispose() => ClearWarnings();
}
