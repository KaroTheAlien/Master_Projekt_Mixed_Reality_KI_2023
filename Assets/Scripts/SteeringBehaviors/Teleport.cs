using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : SteeringBehaviour
{
    protected float teleportDistance;

    public override void Initialize(Transform agentTransform, Transform targetTransform, float maxSpeed, float desiredDistance)
    {
        base.Initialize(agentTransform, targetTransform, maxSpeed, desiredDistance);
        teleportDistance = 10f;
    }

    public override Vector3 CalculateSteeringForce()
    {
        float distanceToTarget = Vector3.Distance(agentTransform.position, targetTransform.position);

        if (distanceToTarget > teleportDistance)
        {
            agentTransform.position = targetTransform.position;
            return Vector3.zero;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
