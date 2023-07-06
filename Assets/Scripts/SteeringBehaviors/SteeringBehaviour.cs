using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    protected Transform agentTransform;     // Reference to the agent's transform
    protected Transform targetTransform;    // Reference to the target's transform
    protected float maxSpeed;               // Maximum speed of the agent
    protected float desiredDistance;        // Desired distance from the target

    public virtual void Initialize(Transform agentTransform, Transform targetTransform, float maxSpeed, float desiredDistance)
    {
        this.agentTransform = agentTransform;
        this.targetTransform = targetTransform;
        this.maxSpeed = maxSpeed;
        this.desiredDistance = desiredDistance;
    }

    public virtual Vector3 CalculateSteeringForce()
    {
        return Vector3.zero;
    }
}
