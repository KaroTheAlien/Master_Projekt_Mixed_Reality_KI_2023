using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "AttackLeft", menuName = "UtilityAI/Actions/Directions/AAttackLeft")]

    public class AAttackLeft : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.Attack(AttackDirection.Left);
        }
    }
}
