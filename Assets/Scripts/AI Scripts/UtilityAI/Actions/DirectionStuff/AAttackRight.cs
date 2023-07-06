using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "AttackRight", menuName = "UtilityAI/Actions/Directions/AAttackRight")]

    public class AAttackRight : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.Attack(AttackDirection.Right);
        }
    }
}
