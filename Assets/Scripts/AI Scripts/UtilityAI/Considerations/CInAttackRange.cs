using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InAttackRange?", menuName = "UtilityAI/Considerations/InAttackRange?")]

public class CInAttackRange : Consideration
{
    public bool reverseOutput = false; // nur true f�r zb: StopMovement!
    public override float ScoreConsideration(Enemy npcAiController)
    {
        //float distance = Vector3.Distance(npcAiController.player.transform.position, npcAiController.transform.position);
        //if(reverseOutput == false)
        //{
        //    if (distance <= 2.15)
        //        Score = 1.0f;
        //    else
        //        Score = 0.0f;
        //
        //}
        ////nicht in attack range? -> gut f�r StopMovement und Go To Player
        //else if (reverseOutput == true)
        //{
        //    if (distance > 2.15)
        //        Score = 1.0f;
        //    else
        //        Score = 0.0f;
        //}
        if (npcAiController.CurrentTargetSpot != null)
        {

            float distance = Vector2.Distance(new Vector2(npcAiController.CurrentTargetSpot.transform.position.x, npcAiController.CurrentTargetSpot.transform.position.z),
                                              new Vector2(npcAiController.transform.position.x, npcAiController.transform.position.z));
            Debug.Log("distance" + distance);
            //f�r in attack range:
            if (reverseOutput == false)
            {
                //Debug.Log("F�r in Attack Range: 0 GUT  Distance zum CurrentTargetSpot: " + distance);
                if (distance <= 0.1)
                    Score = 1.0f;
                else
                    Score = 0.0f;

            }
            //nicht in attack range? -> gut f�r  Go To Player
            else if (reverseOutput == true)
            {
                
                //Debug.Log("F�r nicht in Attack Range: 0 Schlecht -> bewegung sollte aufh�ren  Distance zum CurrentTargetSpot: " + distance);

                if (distance > 0.1)
                    Score = 1.0f;
                else
                    Score = 0.0f;
            }

        }
        else
            Score = 0.0f;
        return Score;

    }


}
