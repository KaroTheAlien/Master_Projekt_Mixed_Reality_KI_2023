using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "DistanceToFightingSpot", menuName = "UtilityAI/Considerations/DistanceToFightingSpot")]

public class CDistanceToSpot : Consideration
{

    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] public bool bRightSpot = false;
    [SerializeField] public bool bMiddleSpot = false;
    [SerializeField] public bool bLeftSpot = false;

    public override float ScoreConsideration(Enemy npcAiController)
    {
        GameObject Spot = null;
        if (bRightSpot)
            Spot = npcAiController.FightSpotController.GetSpot(FightSpotPosition.Right);
        else if (bMiddleSpot)
            Spot = npcAiController.FightSpotController.GetSpot(FightSpotPosition.Middle);
        else if (bLeftSpot)
            Spot = npcAiController.FightSpotController.GetSpot(FightSpotPosition.Left);
        if (Spot is not null)
        {



            //float distance = Vector3.Distance(npcAiController.transform.position,Spot.transform.position);
            float distance = Vector2.Distance(new Vector2(Spot.transform.position.x, Spot.transform.position.z),
                                                  new Vector2(npcAiController.transform.position.x, npcAiController.transform.position.z));
            //Debug.Log("Distanz von Consideation: DistanceToSpot" + Spot.name + Spot.transform.position.ToString() + "Distanz:: " + distance.ToString() );
            float normalizedDistacne = distance / 10; //TODO als funtkion für enemy.getDistanceToPlayer? und dort drin MAX SICHTWEITE -> Graph anpassen
            Score = animationCurve.Evaluate(normalizedDistacne);
            //Debug.Log("Score: " + Score.ToString());
        }
        else
            Score = -1;
        return Score;
    }

}
