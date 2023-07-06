using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
[CreateAssetMenu(fileName = "AttackHeavy", menuName = "UtilityAI/Actions/AttackHeavy")]

    public class AAttackHeavy : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.AttackHeavy();
        }
    }

}