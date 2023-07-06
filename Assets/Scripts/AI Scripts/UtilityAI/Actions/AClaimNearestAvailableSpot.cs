using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "ClaimNearestAvailableSpot", menuName = "UtilityAI/Actions/ClaimNearestAvailableSpot")]

    public class AClaimNearestAvailableSpot : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.ClaimNearestAvailableSpot();
        }
    }
}