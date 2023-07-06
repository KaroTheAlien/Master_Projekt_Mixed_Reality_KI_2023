using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DirectionDrivenSteeringBehaviours : MonoBehaviour
{
    protected static Vector3 currDirection;

    protected static float orientation;

    private void Start()
    {
        orientation = 0;
    }
    public abstract Vector3 CalculateDirection();
    private void FixedUpdate()
    {
        this.transform.position += CalculateDirection();
    }
}
