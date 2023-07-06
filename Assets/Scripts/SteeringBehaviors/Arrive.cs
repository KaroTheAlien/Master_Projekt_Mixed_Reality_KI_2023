using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour
{
    public override void Initialize(Transform agentTransform, Transform targetTransform, float maxSpeed, float desiredDistance)
    {
        base.Initialize(agentTransform, targetTransform, maxSpeed, desiredDistance);
        this.desiredDistance = desiredDistance;
    }

    // Makes the agent slow down when it gets close to the target
    public override Vector3 CalculateSteeringForce()
    {
        Vector3 desiredVelocity = targetTransform.position - agentTransform.position;
        float distance = desiredVelocity.magnitude;

        if (distance > desiredDistance)
        {
            desiredVelocity = desiredVelocity.normalized * maxSpeed;
        }
        else
        {
            desiredVelocity = Vector3.zero;
        }

        return desiredVelocity - agentTransform.forward;
    }
}

