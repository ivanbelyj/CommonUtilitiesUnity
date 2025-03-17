using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public struct RadiusDetectorEventArgs
{
    public IEnumerable<(GameObject, float)> objectsWithDistance;
}

public struct DetectedObjectInfo
{
    public GameObject gameObject;
    public float distance;
}

public class RadiusDetector
{
    public Dictionary<GameObject, float> DistancesByLastObjectDetected { get; private set; } = new();
    public event EventHandler<RadiusDetectorEventArgs> ObjectsEnterRadius;
    public event EventHandler<RadiusDetectorEventArgs> ObjectsLeaveRadius;

    public RadiusDetectorTargetArea TargetArea { get; }
    
    private IReadOnlyList<IRadiusDetectorFilter> filters;

    public RadiusDetector(
        RadiusDetectorTargetArea targetArea,
        IReadOnlyList<IRadiusDetectorFilter> filters)
    {
        TargetArea = targetArea;
        this.filters = filters;
    }

    public void Detect(IEnumerable<DetectedObjectInfo> allObjects)
    {
        var (newInRadius, objectsInRadiusSet) = GetNewObjectsInRadius(allObjects);
        newInRadius.ForEach(x => DistancesByLastObjectDetected.Add(x.gameObject, x.distance));

        var outOfRadius = GetAndRemoveObjectsOutOfRadius(objectsInRadiusSet);

        HandleDispatchEvents(newInRadius, outOfRadius);
    }

    public bool IsDetectedInTargetArea(GameObject gameObject) {
        return DistancesByLastObjectDetected.ContainsKey(gameObject);
    }

    private (List<DetectedObjectInfo> newInRadius, HashSet<GameObject> objectsInRadiusSet)
        GetNewObjectsInRadius(IEnumerable<DetectedObjectInfo> objectsInRadius)
    {
        // Collect new objects in the radius
        var newInRadius = new List<DetectedObjectInfo>();
        var objectsInRadiusSet = new HashSet<GameObject>();
        foreach (var detectedObject in FilterDetected(objectsInRadius))
        {
            objectsInRadiusSet.Add(detectedObject.gameObject);

            if (!DistancesByLastObjectDetected.ContainsKey(detectedObject.gameObject))
            {
                newInRadius.Add(detectedObject);
            }
        }
        return (newInRadius, objectsInRadiusSet);
    }

    private IEnumerable<DetectedObjectInfo> FilterDetected(
        IEnumerable<DetectedObjectInfo> objects)
    {
        var filteredObjects = objects.Where(x => TargetArea.Satisfies(x.distance));
        if (filters == null || !filters.Any())
        {
            return filteredObjects;
        }
        foreach (var filter in filters)
        {
            filteredObjects = filter.FilterDetected(filteredObjects);
        }
        return filteredObjects;
    }

    /// <summary>
    /// Objects out of radius with their distances
    /// </summary>
    private List<KeyValuePair<GameObject, float>> GetAndRemoveObjectsOutOfRadius(
        HashSet<GameObject> objectsInRadiusSet)
    {
        // Collect objects that were in the radius, but now have gone
        var outOfRadius = DistancesByLastObjectDetected
            .Where(x => !objectsInRadiusSet.Contains(x.Key))
            .ToList();

        foreach (var (go, distance) in outOfRadius)
        {
            DistancesByLastObjectDetected.Remove(go);
        }

        return outOfRadius;
    }

    private void HandleDispatchEvents(
        List<DetectedObjectInfo> newInRadius,
        List<KeyValuePair<GameObject, float>> outOfRadius)
    {
        if (outOfRadius.Count > 0)
        {
            InvokeEvent(
                ObjectsLeaveRadius,
                outOfRadius.Select(x => (x.Key, x.Value)).ToList());
        }
            
        if (newInRadius.Count > 0)
        {
            InvokeEvent(
                ObjectsEnterRadius,
                newInRadius.Select(x => (x.gameObject, x.distance)).ToList());
        }

        void InvokeEvent(
            EventHandler<RadiusDetectorEventArgs> @event,
            List<(GameObject, float)> list)
        {
            @event?.Invoke(this, new() {
                objectsWithDistance = list
            });
        }
    }
}
