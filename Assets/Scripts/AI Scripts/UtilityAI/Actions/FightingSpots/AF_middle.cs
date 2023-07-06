using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "middleSpot", menuName = "UtilityAI/Actions/FightingSpots/middleSpot")]

    public class AF_middle : Action
    {

        public override void Execute(Enemy npcAIController)
        {
           // FightSpotController controller = GameObject.FindObjectOfType<FightSpotController>(); // Finde den FightSpotController
           // GameObject MidSpot = controller.getMiddleSpot(); // Hole den LeftSpot vom Controller
           //
           // Debug.Log("Af_Mid: " + MidSpot.transform.position.ToString());


            npcAIController.AddSpotToPool(FightSpotPosition.Middle);
        }
    }
}