using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
[CreateAssetMenu(fileName = "MoveToPlayerSweetSpot", menuName = "UtilityAI/Actions/MoveToPlayerSweetSpot")]
    public class AMoveToPlayerSweetSpot : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.MoveToPlayerSweetSpot();
        }
    }
}