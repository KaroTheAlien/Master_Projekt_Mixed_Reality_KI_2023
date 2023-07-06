using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "AttackBottomLeft", menuName = "UtilityAI/Actions/Directions/AAttackBottomLeft")]

    public class AAttackBottomLeft : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.Attack(AttackDirection.DownLeft);
        }
    }
}
