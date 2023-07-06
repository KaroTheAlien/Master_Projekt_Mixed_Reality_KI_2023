using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{

    [CreateAssetMenu(fileName = "AttackFast", menuName = "UtilityAI/Actions/AttackFast")]
    public class AAttackFast : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.AttackFast();
        }
    }

}