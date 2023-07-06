using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour
{
    // Ensures that the agent moves to the target
    public override Vector3 CalculateSteeringForce()
    {
        Vector3 desiredVelocity = targetTransform.position - agentTransform.position;
        return desiredVelocity.normalized * maxSpeed - agentTransform.forward;
    }
}

