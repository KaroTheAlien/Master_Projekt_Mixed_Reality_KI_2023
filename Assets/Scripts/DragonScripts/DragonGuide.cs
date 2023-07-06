using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class DragonGuide : MonoBehaviour
{

    public List<GameObject> roomScanners;
    private GameObject player;
    public List<GameObject> wayPoints;

    DragonStateController dragonStateController;
    DragonBehaviourController dragonBehaviourController;

    public GameObject blueFireBall;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        dragonStateController = GetComponent<DragonStateController>();
        dragonBehaviourController = GetComponent<DragonBehaviourController>();


        roomScanners = new List<GameObject>();
        wayPoints = new List<GameObject>();
        ListOfRoomScanners();
    }

    private void Update()
    {
        ListOfRoomScanners();
        GetNearestRoomScanner();
    }

    // Method to get the nearest RoomScanner object to the player
    private void GetNearestRoomScanner()
    {
        GameObject nearestScanner = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject scanner in roomScanners)
        {
            float distance = Vector3.Distance(player.transform.position, scanner.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestScanner = scanner;
            }
        }

        // Check if nearestScanner is not null and has no enemies
        if (nearestScanner != null && nearestScanner.GetComponent<RoomScanner>().enemiesInRoom.Count == 0 && dragonStateController.currentSupportState != DragonStateController.States.Heal)
        {
            StartCoroutine(CheckEnemiesCoroutine(nearestScanner));
        }
    }

    // if no enemies are in the room, wait for 5 seconds before start guiding the player (in case enemies should still spawn)
    private IEnumerator CheckEnemiesCoroutine(GameObject scanner)
    {
        yield return new WaitForSeconds(3f);

        if (scanner != null && scanner.GetComponent<RoomScanner>().enemiesInRoom.Count == 0)
        {
            wayPoints = scanner.GetComponent<RoomScanner>().nearestWayPoints;

            // check if the waypoints have already been used
            bool canGuide = scanner.GetComponent<RoomScanner>().wayPointFlags[wayPoints[0]] == false && scanner.GetComponent<RoomScanner>().wayPointFlags[wayPoints[1]] == false;

            if (canGuide)
            {
                dragonStateController.SetSupportState(DragonStateController.States.Guide);
                StartCoroutine(AttackWayPointCoroutine());

                // set the waypoints to true so that they are not used again
                foreach (GameObject wayPoint in scanner.GetComponent<RoomScanner>().wayPoints)
                {
                    scanner.GetComponent<RoomScanner>().wayPointFlags[wayPoint] = true;
                }
            }
        }
    }

    // List of all RoomScanners in the game
    void ListOfRoomScanners()
    {
        roomScanners.RemoveAll(scanner => scanner == null || !scanner.activeSelf);

        foreach (GameObject scanner in GameObject.FindGameObjectsWithTag("RoomScanner"))
        {
            if (!roomScanners.Contains(scanner))
            {
                roomScanners.Add(scanner);
            }
        }
    }

    // Attack both waypoint torches in the room one after the other
    private IEnumerator AttackWayPointCoroutine()
    {
        GuidePlayer(wayPoints[0].transform);

        yield return new WaitForSeconds(2f);

        ChangeTorchColor(wayPoints[0].transform, true);
        GuidePlayer(wayPoints[1].transform);

        yield return new WaitForSeconds(2f);

        ChangeTorchColor(wayPoints[1].transform, true);
        dragonStateController.SetSupportState(DragonStateController.States.Follow);

    }

    public void GuidePlayer(Transform target)
    {
        dragonBehaviourController.SetTarget(target);
    }

    // switch the particle effect and light of the waypoint torch
    private void ChangeTorchColor(Transform wayPoint, bool activateBlueFire)
    {
        Transform torchFire = wayPoint.transform.Find("TorchFire");
        Transform torchFireBlue = wayPoint.transform.Find("TorchFireBlue");

        if (torchFire != null)
        {
            torchFire.gameObject.SetActive(!activateBlueFire);
        }

        if (torchFireBlue != null)
        {
            torchFireBlue.gameObject.SetActive(activateBlueFire);
        }
    }
}
