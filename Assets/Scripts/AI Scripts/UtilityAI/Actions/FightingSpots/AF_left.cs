using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "leftSpot", menuName = "UtilityAI/Actions/FightingSpots/leftSpot")]

    public class AF_left : Action
    {
        public override void Execute(Enemy npcAIController)
        {
           // FightSpotController controller = GameObject.FindObjectOfType<FightSpotController>(); // Finde den FightSpotController
           // GameObject LeftSpot = controller.getLeftSpot(); // Hole den LeftSpot vom Controller
           //
           // Debug.Log("AF_LEFT: " + LeftSpot.transform.position.ToString());

            npcAIController.AddSpotToPool(FightSpotPosition.Left);
        }
    }
}