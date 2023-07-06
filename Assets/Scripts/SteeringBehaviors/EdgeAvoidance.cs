using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeAvoidance : DirectionDrivenSteeringBehaviours
{
    Vector3 edgeVectorLeft;
    Vector3 edgeVectorRight;
    [SerializeField] bool drawRays = false;
    private void Start()
    {
        edgeVectorLeft = new Vector3(1, 0, 1);

    }

    // Avoide getting Trapped in a Corner 
    // 2 Rays, 1 Left 1 Right, if both hit a wall => change Direction 
    public override Vector3 CalculateDirection()
    {
        Vector2 convertedDirection = new Vector2(currDirection.x, currDirection.z);
        Vector2 perpendicularVec = Vector2.Perpendicular(convertedDirection);

        Vector3 convertedPerpendicular = new Vector3(perpendicularVec.x, 0, perpendicularVec.y);
        convertedPerpendicular = Vector3.Normalize(convertedPerpendicular);

        edgeVectorLeft = this.transform.position - convertedPerpendicular;
        edgeVectorRight = this.transform.position + convertedPerpendicular;

        Vector3 directionLeft = Quaternion.Euler(0,30,0) * currDirection;
        Vector3 directionRight = Quaternion.Euler(0, -30, 0) * currDirection;

        if (drawRays)
        {
            Debug.DrawRay(edgeVectorLeft, directionLeft * 20, Color.red);
            Debug.DrawRay(edgeVectorRight, directionRight * 20, Color.green);
            Debug.DrawRay(this.transform.position, currDirection * 100, Color.blue);
        }

        Ray leftRay = new Ray(edgeVectorLeft, directionLeft);
        Ray RightRay = new Ray(edgeVectorRight, directionRight);

        RaycastHit hitLeft;
        RaycastHit hitRight;

        if(Physics.Raycast(leftRay, out hitLeft,1) && Physics.Raycast(RightRay,out hitRight,1)){
            float rand = Random.Range(-0.3f, -1.7f);
            currDirection = -1*directionLeft;
        }
        return currDirection;
    }
}
