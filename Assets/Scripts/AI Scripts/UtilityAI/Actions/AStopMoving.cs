using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "StopMoving", menuName = "UtilityAI/Actions/StopMoving")]
    public class AStopMoving : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.StopMoving();
        }
    }
}
