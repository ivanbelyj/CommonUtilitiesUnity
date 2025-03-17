using UnityEngine;

public class RadiusDetectingComponent2D : RadiusDetectingComponent
{
    protected override IRadiusScanner CreateRadiusScanner()
    {
        return new RadiusScanner2D();
    }
}
