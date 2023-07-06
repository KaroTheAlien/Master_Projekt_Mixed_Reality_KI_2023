using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DistanceToPlayerFightingSpot", menuName = "UtilityAI/Considerations/DistanceToPlayerFightingSpot")]

public class CDistanceToPlayerFightingSpot : Consideration
{
    [SerializeField] private AnimationCurve animationCurve;

    public override float ScoreConsideration(Enemy npcAiController)
    {
        if (npcAiController.CurrentTargetSpot == null)
        {
            Score = 0;
            return Score;
        }
        //float distance = Vector3.Distance(npcAiController.player.transform.position,npcAiController.transform.position);
        float distance = Vector2.Distance(new Vector2(npcAiController.CurrentTargetSpot.transform.position.x, npcAiController.CurrentTargetSpot.transform.position.z),
                                              new Vector2(npcAiController.transform.position.x, npcAiController.transform.position.z));

        float normalizedDistacne = distance / 10; //TODO als funtkion f�r enemy.getDistanceToPlayer? und dort drin MAX SICHTWEITE -> Graph anpassen

        Score = animationCurve.Evaluate(normalizedDistacne) ;
        //Debug.Log(" distacne: " + distance + " normalisiert: " + normalizedDistacne + "Score " + Score );
        if (Score < 0.01) Score = 0;
        return Score;
    }

}
