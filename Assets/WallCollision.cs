using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    // Start is called before the first frame update
    Enemy EnemyOnSpot;
    public Enemy blocker;
    FightSpotController controller;
    public GameObject blockedSpot;
    private int collidersEntered = 0;
    
    void Start()
    {
        controller = FindObjectOfType<FightSpotController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
       
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogError("otherCOllider:" + other);
        if (other.gameObject.tag.Equals("Wall"))
        {
            //if (blockedSpot)
            //    return;
            //this.gameObject.SetActive(false);
            this.collidersEntered++;

            Dictionary<FightSpotPosition,Enemy>.ValueCollection enemies = controller._occupiedSpots.Values;
            var spots = controller._occupiedSpots;
            if(this.gameObject.name.Equals("RightSpot"))
            {
                spots.TryGetValue(FightSpotPosition.Right, out EnemyOnSpot);
                //Debug.LogError("RIP RECHTS");


            }
            else if (this.gameObject.name.Equals("MiddleSpot"))
            {
                spots.TryGetValue(FightSpotPosition.Middle, out EnemyOnSpot);
                //Debug.LogError("RIP MITTE");

            }
            else if (this.gameObject.name.Equals("LeftSpot"))
            {
                spots.TryGetValue(FightSpotPosition.Left, out EnemyOnSpot);
                //Debug.LogError("RIP LINKS");

            }

          
            if (this.gameObject.name.Equals("RightSpot"))
            {
                blockedSpot = blocker.FightSpotController.ClaimSpot2(blocker, FightSpotPosition.Right);

                //Debug.LogError("blocker right claimed");

            }
            else if (this.gameObject.name.Equals("MiddleSpot"))
            {
                blockedSpot = blocker.FightSpotController.ClaimSpot2(blocker, FightSpotPosition.Middle);
                //Debug.LogError("blocker mid claimed")


            }
            else if (this.gameObject.name.Equals("LeftSpot"))
            {
                blockedSpot=  blocker.FightSpotController.ClaimSpot2(blocker, FightSpotPosition.Left);
                //Debug.LogError("blocker left claimed");

            }
            if (EnemyOnSpot == null)
                return;
            //EnemyOnSpot.FightSpotController.ReleaseSpot(EnemyOnSpot);
            EnemyOnSpot.StopMoving2();
            EnemyOnSpot.navAgent.isStopped = true;
            EnemyOnSpot.navAgent.ResetPath();
            EnemyOnSpot.navAgent.destination = EnemyOnSpot.gameObject.transform.position;

            EnemyOnSpot.bAlreadyClaimedSpot = false;
            EnemyOnSpot.bFightingSpotsCalulated = false;
            EnemyOnSpot.AfterFinishedAction();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Wall"))
        {
            this.collidersEntered--;
            if (blockedSpot == null || this.collidersEntered != 0)
                return;
            if (this.gameObject.name.Equals("RightSpot"))
            {
                blocker.FightSpotController.ReleaseSpot(blocker);
                //Debug.LogError("blocker right rel");

            }
            if (this.gameObject.name.Equals("MiddleSpot"))
            {
                blocker.FightSpotController.ReleaseSpot(blocker);
                //Debug.LogError("blocker mid rel");


            }
            if (this.gameObject.name.Equals("LeftSpot"))
            {
                blocker.FightSpotController.ReleaseSpot(blocker);
                //Debug.LogError("blocker left rel");

            }
            blockedSpot = null;

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
        {
            this.gameObject.SetActive(true);

        }
    }

}
