using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightSpotController : MonoBehaviour
{
    public GameObject rightSpot;
    public GameObject middleSpot;
    public GameObject leftSpot;
    public new GameObject camera;
    public GameObject spotHolder;
    private bool orientationLocked => this._occupiedSpots.Count > 0;
    [SerializeField] private int enemyLimit = 2; // max 3

    public readonly Dictionary<FightSpotPosition, Enemy> _occupiedSpots = new();


    Enemy currentEnemy;
//    private void Start()
//    {
//     // Erstelle ein temporäres Array mit den Gameobjects
//     GameObject[] gameObjSpots = { this.rightSpot, this.middleSpot, this.leftSpot };
//
//     foreach (GameObject spot in gameObjSpots)
//         Debug.Log(" meine Spots:" + spot.name + " position:" + spot.transform.position);
//    }

    private void Update()
    {
        // skip reorientation if spots are claimed
        if (this.orientationLocked) return;

        Vector3 cameraRotation = this.camera.transform.eulerAngles;
        Vector3 spotRotation = this.spotHolder.transform.eulerAngles;
        float rotationDifference = Math.Abs(cameraRotation.y - spotRotation.y);
        // skip reorientation if difference < 45°
        if (rotationDifference < 45) return;
        
        float newRotation = Convert.ToInt32(cameraRotation.y / 45f) * 45f;
        this.spotHolder.transform.eulerAngles = new Vector3(spotRotation.x, newRotation, spotRotation.z);
    }

    public GameObject ClaimSpot(Enemy enemy, IEnumerable<FightSpotPosition> preferredSpots)
    {
        // can not claim spot if limit is reached
        int occupiedSpots = 0;
        for (int i = 0; i < this._occupiedSpots.Count; i++)
        {
            if (this._occupiedSpots.Values.ToList()[i].tag != "Blocker")
            {
                occupiedSpots++;
            }
        }
        if (this._occupiedSpots.Count >= 3 || occupiedSpots >= this.enemyLimit) return null;

        // select spot position with default fallback of middle spot, if available
        FightSpotPosition spotPosition;
        try
        {
            spotPosition = this._occupiedSpots.ContainsKey(FightSpotPosition.Middle) == false
            ? FightSpotPosition.Middle
            : preferredSpots.First(spotPos => this._occupiedSpots.ContainsKey(spotPos) == false);
        } catch (Exception e) { spotPosition = FightSpotPosition.Middle; }
        // claim spot position
        this._occupiedSpots[spotPosition] = enemy;
        GameObject spot = this.GetSpot(spotPosition);

        // adjust to attack range
        Vector3 adjustedPosition = spot.transform.localPosition;
        adjustedPosition.z += enemy.attackRange;
        spot.transform.localPosition = adjustedPosition;
        
        return spot;
    }
    public GameObject ClaimSpot2(Enemy enemy, FightSpotPosition fight_spot)
    {
        // select spot position with default fallback of middle spot, if available
        FightSpotPosition spotPosition = fight_spot;// claim spot position
        this._occupiedSpots[spotPosition] = enemy;
        GameObject spot = this.GetSpot(spotPosition);

        // adjust to attack range
        Vector3 adjustedPosition = spot.transform.localPosition;
        adjustedPosition.z += enemy.attackRange;
        spot.transform.localPosition = adjustedPosition;

        return spot;
    }
    public void ReleaseSpot(Enemy enemy)
    {
        List<FightSpotPosition> occupiedSpots = this._occupiedSpots.Keys
            .Where(spot => this._occupiedSpots[spot] == enemy)
            .ToList();
        foreach (FightSpotPosition spot in occupiedSpots)
        {
            this.GetSpot(spot).transform.localPosition = Vector3.zero;
            this._occupiedSpots.Remove(spot);
        }
    }

    public GameObject GetSpot(FightSpotPosition position)
    {
        return position switch
        {
            FightSpotPosition.Right => this.rightSpot,
            FightSpotPosition.Middle => this.middleSpot,
            FightSpotPosition.Left => this.leftSpot,
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
        };
    }
}
