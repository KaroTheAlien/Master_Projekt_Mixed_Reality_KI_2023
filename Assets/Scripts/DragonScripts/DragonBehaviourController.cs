using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class DragonBehaviourController : MonoBehaviour
{
    [Header("Steering Behaviour Variables")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float desiredDistance;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float desiredHeight;

    private Rigidbody dragonRigidbody;
    private SteeringBehaviour[] behaviorComponents;

    [Header("Wander/Distract Enemy")]
    public bool wandering = false;
    private Transform wanderTarget;
    private bool isWandering = false;
    private WaitForSeconds wanderDelay = new WaitForSeconds(2f);

    private void Start()
    {
        dragonRigidbody = GetComponent<Rigidbody>();
        behaviorComponents = GetComponents<SteeringBehaviour>();

        // Initialize each SteeringBehaviour component with the appropriate parameters
        foreach (SteeringBehaviour behavior in behaviorComponents)
        {
            behavior.Initialize(transform, targetTransform, maxSpeed, desiredDistance);
        }
    }

    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }

    private void Update()
    {
        //if (wandering)
        //{
        //    Wander();
        //}

        // Calculate the total steering force by summing up the forces from each SteeringBehaviour component
        Vector3 steeringForce = Vector3.zero;
        foreach (SteeringBehaviour behavior in behaviorComponents)
        {
            steeringForce += behavior.CalculateSteeringForce();
        }

        // Set flying height
        Vector3 desiredPosition = transform.position;
        desiredPosition.y = desiredHeight;
        transform.position = desiredPosition;
        if(targetTransform != null)
            transform.LookAt(targetTransform.position);

        // Apply the calculated steering force to move the dragon
        ApplySteeringForce(steeringForce);
    }

    private void ApplySteeringForce(Vector3 force)
    {
        Vector3 newVelocity = dragonRigidbody.velocity + force * Time.deltaTime;

        if (newVelocity.magnitude > maxSpeed)
        {
            newVelocity = newVelocity.normalized * maxSpeed;
        }

        dragonRigidbody.velocity = newVelocity;

        if (newVelocity.magnitude > 0.1f)
        {
            Quaternion newRotation = Quaternion.LookRotation(newVelocity.normalized);
            dragonRigidbody.MoveRotation(newRotation);
        }
        else
        {
            newVelocity = Vector3.zero;
        }
    }

    private void Wander()
    {
        // Check for obstacles using rays
        bool obstacleInFront = CheckRay(Vector3.forward);
        bool obstacleBehind = CheckRay(Vector3.back);
        bool obstacleLeft = CheckRay(Vector3.left);
        bool obstacleRight = CheckRay(Vector3.right);

        // If there are no obstacles and not currently wandering, start the coroutine to place a new wander target
        if (!obstacleInFront && wanderTarget == null && !isWandering)
        {
            StartCoroutine(PlaceWanderTargetCoroutine(RandomizeDirection()));
        }
        else if (!obstacleLeft && wanderTarget == null && !isWandering)
        {
            StartCoroutine(PlaceWanderTargetCoroutine(RandomizeDirection()));
        }
        else if (!obstacleRight && wanderTarget == null && !isWandering)
        {
            StartCoroutine(PlaceWanderTargetCoroutine(RandomizeDirection()));
        }
        else if (!obstacleBehind && wanderTarget == null && !isWandering)
        {
            StartCoroutine(PlaceWanderTargetCoroutine(RandomizeDirection()));
        }

        // Move towards the wander target
        if (wanderTarget != null)
        {
            Vector3 direction = wanderTarget.position - transform.position;
            if (direction.magnitude > desiredDistance)
            {
                SetTarget(wanderTarget);
            }
        }
    }

    private IEnumerator PlaceWanderTargetCoroutine(Vector3 direction)
    {
        isWandering = true;

        while (true)
        {
            PlaceWanderTarget(direction);

            yield return wanderDelay;
        }
    }

    private bool CheckRay(Vector3 direction)
    {
        float rayDistance = desiredDistance + 1f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player") || hit.collider.CompareTag("Untagged"))
            {
                return true;
            }
        }
        return false;
    }

    // place a new wandertarget
    private void PlaceWanderTarget(Vector3 direction)
    {
        float targetDistance = desiredDistance + 1f;
        Vector3 targetPosition = transform.position + transform.TransformDirection(direction) * targetDistance;

        GameObject targetObject = new GameObject("WanderTarget");
        targetObject.transform.position = targetPosition;

        Destroy(wanderTarget?.gameObject);
        wanderTarget = targetObject.transform;
    }

    // place the wandertarget in random directions
    private Vector3 RandomizeDirection()
    {
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        return directions[Random.Range(0, directions.Length)];
    }
}

