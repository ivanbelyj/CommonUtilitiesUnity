using System;
using UnityEngine;

[Serializable]
public class CompositeRadiusDetectorConfiguration
{
    [Tooltip("Update will be called every frame if this field is set to 'false'")]
    [SerializeField]
    private bool useUpdateFrequency = true;
    public bool UseUpdateFrequency => useUpdateFrequency;

    [SerializeField]
    private float updateFrequencySeconds = 0.2f;
    public float UpdateFrequencySeconds => updateFrequencySeconds;
    
    [Header("Debug")]
    [SerializeField]
    private bool allowDrawGizmos = false;
    public bool AllowDrawGizmos => allowDrawGizmos;
}
