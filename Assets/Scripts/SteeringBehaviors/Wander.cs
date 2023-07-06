using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 2f;
    [SerializeField] private float minTimeBetweenWanders = 2f;
    [SerializeField] private LayerMask obstacleMask;

    private Transform wanderTarget;
    private float timeSinceLastWander;
    private bool isWandering;
    DragonStateController dragonStateController;

    private void Start()
    {
        wanderTarget = new GameObject("WanderTarget").transform;
        isWandering = false;
    }

    private void Update()
    {
        if (isWandering)
        {
            timeSinceLastWander += Time.deltaTime;

            if (timeSinceLastWander >= minTimeBetweenWanders)
            {
                SetNewWanderTarget();
            }
        }
    }

    private void SetNewWanderTarget()
    {
        // Find a random direction without obstacles
        Vector3 randomDirection = GetRandomDirectionWithoutObstacles();
        wanderTarget.position = transform.position + randomDirection * GetRandomWanderDistance();
        timeSinceLastWander = 0f;
    }

    private Vector3 GetRandomDirectionWithoutObstacles()
    {
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, randomDirection, out hit, raycastDistance, obstacleMask))
        {
            // If there is an obstacle in the random direction, try again
            return GetRandomDirectionWithoutObstacles();
        }

        return randomDirection.normalized;
    }

    private float GetRandomWanderDistance()
    {
        // Customize this method to return a random distance for wandering
        return Random.Range(2f, 5f);
    }

    public void StartWandering()
    {
        isWandering = true;
        StartCoroutine(WanderingDuration());
    }

    public void StopWandering()
    {
        isWandering = false;
        dragonStateController.SetAttackState(DragonStateController.States.Damage);
    }

    IEnumerator WanderingDuration()
    {
        yield return new WaitForSeconds(10);
        StopWandering();
    }

    private void OnDestroy()
    {
        if (wanderTarget != null)
        {
            Destroy(wanderTarget.gameObject);
        }
    }
}
