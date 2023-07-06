using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "IfSpotClaimed", menuName = "UtilityAI/Considerations/IfSpotClaimed")]

public class CIfSpotClaimed : Consideration
{
    public override float ScoreConsideration(Enemy npcAiController)
    {
        bool alreadyClaimed = npcAiController.getIfAlreadyClaimedSpot();
        if (alreadyClaimed)
            Score = 0;
        else
            Score = 1;


        return Score;
    }

}
