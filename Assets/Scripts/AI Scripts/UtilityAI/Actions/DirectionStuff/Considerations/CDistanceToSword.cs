using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
//using UnityEditor.UIElements;
using UnityEngine;


[CreateAssetMenu(fileName = "DistanceToSword", menuName = "UtilityAI/Considerations/DistanceToSword")]
//f�r angriffsrichtung
public class CDistanceToSword : Consideration
{
    [SerializeField] private AnimationCurve animationCurve;
    //Set to 1 for Right Hand:
    [SerializeField] private int Swordindex = 0;
    //0-7
    [SerializeField] private int ColliderIndex = 0;
     public override float ScoreConsideration(Enemy npcAiController) 
    {

        float distance = Vector3.Distance(npcAiController.PlayerSwords.ElementAt(Swordindex).transform.position, npcAiController.Colliders.ElementAt(ColliderIndex).transform.position);
        //Todo Ordentlich skalieren -> 0 ... 1 
        
        Score = animationCurve.Evaluate(distance);
        //Debug.Log("Distance von Schwert: " + npcAiController.PlayerSwords.ElementAt(Swordindex).name + " zu " + npcAiController.Colliders.ElementAt(ColliderIndex).transform.name + " : " + distance);
        return Score;
    }

}
