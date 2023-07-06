using System.Collections.Generic;
using UnityEngine;

public class RoomScanner : MonoBehaviour
{
    [SerializeField] private float scanRadius = 8f;

    public List<GameObject> enemiesInRoom;
    public List<GameObject> wayPoints;
    [SerializeField] public Dictionary<GameObject, bool> wayPointFlags = new();

    public List<GameObject> nearestWayPoints;

    private void Awake()
    {
        enemiesInRoom = new List<GameObject>();
        wayPoints = new List<GameObject>();
        nearestWayPoints = new List<GameObject>();
        
        UpdateEnemyList();
    }

    public void UpdateWayPointFlags()
    {
        UpdateWayPointList();

        foreach (GameObject wayPoint in wayPoints)
        {
            wayPointFlags.Add(wayPoint, false);
        }
        
        //Debug.Log(this.wayPointFlags.Count);
    }

    private void Update()
    {
        UpdateEnemyList();
        UpdateWayPointList();
        GetNearestWayPoints();
    }

    private void UpdateEnemyList()
    {
        // Remove inactive enemies from the list
        enemiesInRoom.RemoveAll(enemy => enemy == null || !enemy.activeSelf);

        // Add all enemies in the scene to the list
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // Only add the enemy to the list if it is within the scan radius
            if (distance <= scanRadius && !enemiesInRoom.Contains(enemy))
            {
                enemiesInRoom.Add(enemy);
            }
        }
    }

    private void UpdateWayPointList()
    {
        wayPoints.RemoveAll(wayPoint => wayPoint == null || !wayPoint.activeSelf);

        GameObject[] allWayPoints = GameObject.FindGameObjectsWithTag("WayPoint");

        // Add all torches in the scene to the list
        foreach (GameObject wayPoint in allWayPoints)
        {
            if (!wayPoints.Contains(wayPoint) && wayPoint != null)
            {
                wayPoints.Add(wayPoint);
            }
        }

    }

    public void GetNearestWayPoints()
    {
        nearestWayPoints.Clear();
        List<GameObject> sortedWayPoints = new List<GameObject>(wayPoints);

        sortedWayPoints.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        for (int i = 0; i < Mathf.Min(2, sortedWayPoints.Count); i++)
        {
            nearestWayPoints.Add(sortedWayPoints[i]);
        }
    }
}
