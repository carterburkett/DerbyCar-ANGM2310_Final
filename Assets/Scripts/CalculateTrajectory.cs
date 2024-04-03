using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CalculateTrajectory : MonoBehaviour
{
    //I don't know if this is the proper way to do this or not. I'm just using what I learned in Calc 1

    public Vector3 Calculate3DTrajectory(Vector3 startPos, Vector3 destination) {
        Vector3 launchVelocity;

        float distance = Vector3.Distance(startPos, destination);
        float angle = Vector3.Angle(startPos, destination);
        
        float angleRad = Mathf.Deg2Rad * angle;

        float dX = Mathf.Abs(startPos.x - destination.x);
        float dY = Mathf.Abs(startPos.y - destination.y);
        float dZ = Mathf.Abs(startPos.z - destination.z);

        float horizDistance = Mathf.Sqrt(Mathf.Pow(dX,2) + Mathf.Pow(dZ,2));

        float velocity = (horizDistance/Mathf.Cos(angle)) / Mathf.Sqrt(Physics.gravity.magnitude / (2 * horizDistance * Mathf.Tan(angle) - dY));
        
        launchVelocity.y = velocity * Mathf.Sin(angle);
        launchVelocity.x = velocity * Mathf.Cos(angle) * horizDistance;
        launchVelocity.z = launchVelocity.x;
        


        return launchVelocity;
    }
}
