using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompositeRadiusDetector
{
    private readonly IRadiusScanner radiusScanner;
    private readonly CompositeRadiusDetectorConfiguration configuration;
    
    private List<RadiusDetector> detectors = new();
    private float secondsSinceLastUpdate = 0f;
    private bool isScannerInitialized = false;

    public CompositeRadiusDetector(
        IRadiusScanner radiusScanner,
        CompositeRadiusDetectorConfiguration configuration)
    {
        this.radiusScanner = radiusScanner;
        this.configuration = configuration;
    }

    public bool ShouldTick() => detectors.Count > 0;

    public void Tick(Vector3 originPosition, bool drawGizmos)
    {
        secondsSinceLastUpdate += Time.deltaTime;

        if (!configuration.UseUpdateFrequency
            || secondsSinceLastUpdate >= configuration.UpdateFrequencySeconds)
        {
            Detect(originPosition, drawGizmos);
            secondsSinceLastUpdate = 0;
        }
    }

    /// <summary>
    /// Adds and returns detector that monitors specified radius
    /// </summary>
    public RadiusDetector AddDetector(
        RadiusDetectorTargetArea targetArea,
        int layerMask,
        EventHandler<RadiusDetectorEventArgs> onObjectsEnterRadius,
        EventHandler<RadiusDetectorEventArgs> onObjectsLeaveRadius,
        IReadOnlyList<IRadiusDetectorFilter> filters = null)
    {
        EnsureScannerIsInitialized(targetArea.MaxRadius);
        radiusScanner.LayerMask |= layerMask;

        var detector = new RadiusDetector(targetArea, filters);
        detector.ObjectsEnterRadius += onObjectsEnterRadius;
        detector.ObjectsLeaveRadius += onObjectsLeaveRadius;
        detectors.Add(detector);

        detector.TargetArea.MaxRadiusChanged += (_, _) => UpdateScannerRadius();

        UpdateScannerRadius();
        return detector;
    }

    private void Detect(Vector3 originPosition, bool drawGizmos)
    {
        var objectsInRadius = radiusScanner.Scan(originPosition, drawGizmos);
        foreach (var detector in detectors)
        {
            detector.Detect(objectsInRadius);
        }
    }

    private void EnsureScannerIsInitialized(float radius)
    {
        if (!isScannerInitialized)
        {
            radiusScanner.Radius = radius;
            radiusScanner.AllowDrawGizmos = configuration.AllowDrawGizmos;
            isScannerInitialized = true;
        }
    }

    private void UpdateScannerRadius()
    {
        radiusScanner.Radius = detectors.Max(x => x.TargetArea.MaxRadius);
    }
}
