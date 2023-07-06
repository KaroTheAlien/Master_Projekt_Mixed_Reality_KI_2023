using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : SteeringBehaviour
{
    [SerializeField] protected LayerMask obstacleLayer;     // Layer mask to filter obstacles
    [SerializeField] protected float avoidDistance;         // Distance to start avoiding obstacles

    public void Initialize(Transform agentTransform, Transform targetTransform, float maxSpeed, float desiredDistance, LayerMask obstacleLayer, float avoidDistance)
    {
        base.Initialize(agentTransform, targetTransform, maxSpeed, desiredDistance);
        this.obstacleLayer = obstacleLayer;
        this.avoidDistance = avoidDistance;
    }

    // Ensures that the agent avoids collisions
    public override Vector3 CalculateSteeringForce()
    {
        Vector3 desiredVelocity = targetTransform.position - agentTransform.position;

        Vector3[] raycastDirections = {
            agentTransform.forward,     // Forward
            -agentTransform.forward,    // Backward
            agentTransform.right,       // Right
            -agentTransform.right,      // Left
            //agentTransform.up,          // Up
            //-agentTransform.up          // Down
        };

        // Check for collisions in each direction
        foreach (Vector3 direction in raycastDirections)
        {
            RaycastHit hit;
            if (Physics.Raycast(agentTransform.position, direction, out hit, avoidDistance, obstacleLayer))
            {
                // Avoid collision by moving in the opposite direction
                return -direction.normalized;
            }
        }

        return desiredVelocity - agentTransform.forward;
    }
}
