using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "rightSpots", menuName = "UtilityAI/Actions/FightingSpots/rightSpot")]

    public class AF_right : Action
    {

        public override void Execute(Enemy npcAIController)
        {
            //FightSpotController controller = GameObject.FindObjectOfType<FightSpotController>(); // Finde den FightSpotController
            //GameObject RightSpot = controller.getMiddleSpot(); // Hole den LeftSpot vom Controller
            //Debug.Log("AF_Right: " + RightSpot.transform.position.ToString());
            npcAIController.AddSpotToPool(FightSpotPosition.Right);
        }
    }
}