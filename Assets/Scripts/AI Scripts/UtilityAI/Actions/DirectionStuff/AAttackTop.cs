using UnityEngine;

namespace UtilityAI
{
    [CreateAssetMenu(fileName = "AttackTop", menuName = "UtilityAI/Actions/Directions/AAttackTop")]

    public class AAttackTop : Action
    {
        public override void Execute(Enemy npcAIController)
        {
            npcAIController.Attack(AttackDirection.UpRightHand);
        }
    }
}
