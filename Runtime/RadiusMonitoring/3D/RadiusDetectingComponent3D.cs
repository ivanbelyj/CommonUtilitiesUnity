using UnityEngine;

public class RadiusDetectingComponent3D : RadiusDetectingComponent
{
    protected override IRadiusScanner CreateRadiusScanner()
    {
        return new RadiusScanner3D();
    }
}
