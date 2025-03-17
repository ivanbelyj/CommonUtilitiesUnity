using System.Collections.Generic;
using UnityEngine;

public interface IRadiusScanner
{
    IEnumerable<DetectedObjectInfo> Scan(Vector3 originPosition, bool drawGizmos);
    float Radius { get; set; }
    int LayerMask { get; set; }
    bool AllowDrawGizmos { get; set; }
}
