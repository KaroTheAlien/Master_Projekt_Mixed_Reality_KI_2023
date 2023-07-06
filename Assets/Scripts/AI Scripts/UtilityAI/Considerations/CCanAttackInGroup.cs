using AI_Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "CanAttackInGroup", menuName = "UtilityAI/Considerations/CanAttackInGroup")]
public class CCanAttackInGroup : Consideration
{
    public override float ScoreConsideration(Enemy npcAiController)
    {
        return GameObject.FindGameObjectWithTag("GroupBehaviourController").GetComponent<EnemyGroupBehaviourController>().CanAttack(npcAiController) ? 1.0f : 0.0f;
    }
}
