using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "AttackBottomRight", menuName = "UtilityAI/Actions/Directions/AAttackBottomRight")]

    public class AAttackBottomRight : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.Attack(AttackDirection.DownRight);
        }
    }
}
