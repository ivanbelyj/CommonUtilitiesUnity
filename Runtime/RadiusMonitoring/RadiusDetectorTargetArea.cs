using System;
using UnityEngine;

public record RadiusDetectorTargetArea
{
    public class MaxRadiusChangedEventArgs
    {
        public float NewValue { get; set; }
        public float OldValue { get; set; }
    }

    private float minRadius;
    private float maxRadius;
    public event EventHandler<MaxRadiusChangedEventArgs> MaxRadiusChanged;

    public float MinRadius
    {
        get => minRadius;
        set
        {
            minRadius = value;
        }
    }

    public float MaxRadius
    {
        get => maxRadius;
        set
        {
            var oldValue = maxRadius;
            maxRadius = value;
            MaxRadiusChanged?.Invoke(this, new() {
                NewValue = maxRadius,
                OldValue = oldValue
            });
        }
    }

    public bool Satisfies(float distance)
    {
        return distance >= MinRadius && distance < MaxRadius;
    }
}
