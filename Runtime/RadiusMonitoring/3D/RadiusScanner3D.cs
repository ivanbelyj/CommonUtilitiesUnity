using System.Collections.Generic;
using UnityEngine;

// AI-Generated
// Todo: review

public class RadiusScanner3D : IRadiusScanner
{
    public float Radius { get; set; }
    public int LayerMask { get; set; }
    public bool AllowDrawGizmos { get; set; }

    private Collider[] overlapBuffer = new Collider[64];
    private DetectedObjectInfo[] resultsArray = new DetectedObjectInfo[64];
    private int lastCount;

    public IEnumerable<DetectedObjectInfo> Scan(Vector3 originPosition, bool drawGizmos)
    {
        lastCount = Physics.OverlapSphereNonAlloc(
            originPosition,
            Radius,
            overlapBuffer,
            LayerMask
        );

        if (resultsArray.Length < lastCount)
        {
            resultsArray = new DetectedObjectInfo[Mathf.NextPowerOfTwo(lastCount)];
        }

        int validCount = 0;
        float radiusSqr = Radius * Radius;

        for (int i = 0; i < lastCount; i++)
        {
            Collider collider = overlapBuffer[i];
            if (collider == null) continue;

            Vector3 objectPosition = collider.transform.position;
            Vector3 direction = objectPosition - originPosition;
            float distanceSqr = direction.sqrMagnitude;

            if (distanceSqr <= radiusSqr)
            {
                float distance = Mathf.Sqrt(distanceSqr);

                if (drawGizmos)
                {
                    HandleDrawGizmos(true, objectPosition, originPosition);
                }

                resultsArray[validCount++] = new DetectedObjectInfo()
                {
                    distance = distance,
                    gameObject = collider.gameObject
                };
            }
            else if (drawGizmos)
            {
                HandleDrawGizmos(false, objectPosition, originPosition);
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

    private void HandleDrawGizmos(
        bool isDistanceLess,
        Vector3 objectPosition,
        Vector3 originPosition)
    {
        if (AllowDrawGizmos)
        {
            Color prevCol = Gizmos.color;
            Gizmos.color = isDistanceLess ? Color.green : Color.red;
            Gizmos.DrawLine(objectPosition, originPosition);
            Gizmos.color = prevCol;
        }
    }

    public void EnsureBufferCapacity(int requiredCapacity)
    {
        if (overlapBuffer.Length < requiredCapacity)
        {
            overlapBuffer = new Collider[Mathf.NextPowerOfTwo(requiredCapacity)];
            resultsArray = new DetectedObjectInfo[Mathf.NextPowerOfTwo(requiredCapacity)];
        }
    }
}
