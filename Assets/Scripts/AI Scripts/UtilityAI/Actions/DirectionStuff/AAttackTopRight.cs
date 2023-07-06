using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "AttackTopRight", menuName = "UtilityAI/Actions/Directions/AAttackTopRight")]

    public class AAttackTopRight : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.Attack(AttackDirection.RightUp);
        }
    }
}
