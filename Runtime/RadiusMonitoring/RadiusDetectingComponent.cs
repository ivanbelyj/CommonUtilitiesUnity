using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Checks for objects in the specified radius and provides according events
/// </summary>
public abstract class RadiusDetectingComponent : MonoBehaviour
{
    [SerializeField]
    private CompositeRadiusDetectorConfiguration lowRadiusAlwaysUpdateConfiguration;

    [SerializeField]
    private CompositeRadiusDetectorConfiguration commonRadiusMediumUpdateConfiguration;

    [SerializeField]
    private CompositeRadiusDetectorConfiguration longRadiusInfrequentUpdateConfiguration;

    private CompositeRadiusDetector lowRadiusAlwaysUpdate;
    private CompositeRadiusDetector commonRadiusMediumUpdate;
    private CompositeRadiusDetector longRadiusInfrequentUpdate;

    private CompositeRadiusDetector[] allCompositeDetectors;

    private void Awake()
    {
        InitializeCompositeDetectors();
    }

    public RadiusDetector AddLowRadiusAlwaysUpdateDetector(
        RadiusDetectorTargetArea targetArea,
        int layerMask,
        EventHandler<RadiusDetectorEventArgs> onObjectsEnterRadius,
        EventHandler<RadiusDetectorEventArgs> onObjectsLeaveRadius,
        IReadOnlyList<IRadiusDetectorFilter> filters = null)
        => AddDetector(lowRadiusAlwaysUpdate, targetArea, layerMask, onObjectsEnterRadius, onObjectsLeaveRadius, filters);

    public RadiusDetector AddCommonRadiusMediumUpdateDetector(
        RadiusDetectorTargetArea targetArea,
        int layerMask,
        EventHandler<RadiusDetectorEventArgs> onObjectsEnterRadius,
        EventHandler<RadiusDetectorEventArgs> onObjectsLeaveRadius,
        IReadOnlyList<IRadiusDetectorFilter> filters = null)
        => AddDetector(commonRadiusMediumUpdate, targetArea, layerMask, onObjectsEnterRadius, onObjectsLeaveRadius, filters);

    public RadiusDetector AddLongRadiusInfrequentUpdateDetector(
        RadiusDetectorTargetArea targetArea,
        int layerMask,
        EventHandler<RadiusDetectorEventArgs> onObjectsEnterRadius,
        EventHandler<RadiusDetectorEventArgs> onObjectsLeaveRadius,
        IReadOnlyList<IRadiusDetectorFilter> filters = null)
        => AddDetector(longRadiusInfrequentUpdate, targetArea, layerMask, onObjectsEnterRadius, onObjectsLeaveRadius, filters);

    private RadiusDetector AddDetector(
        CompositeRadiusDetector compositeDetector,
        RadiusDetectorTargetArea targetArea,
        int layerMask,
        EventHandler<RadiusDetectorEventArgs> onObjectsEnterRadius,
        EventHandler<RadiusDetectorEventArgs> onObjectsLeaveRadius,
        IReadOnlyList<IRadiusDetectorFilter> filters)
        => compositeDetector.AddDetector(targetArea, layerMask, onObjectsEnterRadius, onObjectsLeaveRadius, filters);

    private void InitializeCompositeDetectors()
    {
        lowRadiusAlwaysUpdate = new(CreateRadiusScanner(), lowRadiusAlwaysUpdateConfiguration);
        commonRadiusMediumUpdate = new(CreateRadiusScanner(), commonRadiusMediumUpdateConfiguration);
        longRadiusInfrequentUpdate = new(CreateRadiusScanner(), longRadiusInfrequentUpdateConfiguration);

        allCompositeDetectors = new[] {
            lowRadiusAlwaysUpdate, commonRadiusMediumUpdate, longRadiusInfrequentUpdate
        };
    }

    private void Update()
    {
        HandleUpdate(false);
    }

    private void OnDrawGizmos()
    {
        if (allCompositeDetectors != null)
        {
            HandleUpdate(true);
        }
    }

    protected abstract IRadiusScanner CreateRadiusScanner();

    private void HandleUpdate(bool drawGizmos)
    {
        foreach (var compositeDetector in allCompositeDetectors)
        {
            HandleCompositeDetectorUpdate(compositeDetector, drawGizmos);
        }
    }

    private void HandleCompositeDetectorUpdate(
        CompositeRadiusDetector compositeDetector,
        bool drawGizmos)
    {
        if (compositeDetector.ShouldTick())
        {
            compositeDetector.Tick(transform.position, drawGizmos);
        }
    }
}
