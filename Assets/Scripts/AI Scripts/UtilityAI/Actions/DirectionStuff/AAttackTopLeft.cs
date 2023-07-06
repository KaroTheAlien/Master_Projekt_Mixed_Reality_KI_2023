using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "AttackTopLeft", menuName = "UtilityAI/Actions/Directions/AAttackTopLeft")]

    public class AAttackTopLeft : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.Attack(AttackDirection.LeftUp);
        }
    }
}
