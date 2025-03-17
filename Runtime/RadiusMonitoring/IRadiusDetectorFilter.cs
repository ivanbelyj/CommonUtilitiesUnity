using System.Collections.Generic;
using UnityEngine;

public interface IRadiusDetectorFilter
{
    IEnumerable<DetectedObjectInfo> FilterDetected(IEnumerable<DetectedObjectInfo> objects);
}
