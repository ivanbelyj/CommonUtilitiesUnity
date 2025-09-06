using System.Collections.Generic;
using UnityEngine;

public struct RadiusDetectorEventArgs
{
    public IEnumerable<(GameObject, float)> objectsWithDistance;
}
