using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirection : DirectionDrivenSteeringBehaviours
{
    float movementSpeed;

    public override Vector3 CalculateDirection()
    {
        return currDirection;
    }

    public void SetCurrDircetion(Vector3 dir)
    {
        orientation = Vector3.Angle(currDirection, new Vector3(1,0,0));
        currDirection = dir *movementSpeed;
    }
    public void SetSpeed(float speed)
    {
        movementSpeed = speed;
    }
}
