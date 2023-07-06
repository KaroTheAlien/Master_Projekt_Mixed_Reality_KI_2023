using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI
{
    public class UtilityAI : MonoBehaviour
    {
        //public List<Action> actions = new List<Action>();
        public Action bestAction { get; set; }
        //private NPCAIController npcAiController;
        private Enemy npcAiController;
        public bool bBestActionFound { get; set; }
        private bool bFinishedDeciding { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            //npcAiController = GetComponent<NPCAIController>();
            npcAiController = GetComponent<Enemy>();
            bBestActionFound = false;
            bFinishedDeciding = false;
        }

        // Update is called once per frame
        void Update()
        {
            //if (bFinishedDeciding == false)
            //{
            //    //bFinishedDeciding = true;
            //    CalculateBestAction(npcAiController.actions);
            //}
        }

        public void CalculateBestAction(Action[] availableActions)
        {
            float score = 0.0f;
            int bestActionIndex = 0;
            float calculatedScore = -1.0f;

            if (availableActions.Length > 0)
            {
                for (int i = 0; i < availableActions.Length; i++)
                {
                    calculatedScore = ScoreAction(availableActions[i]);
                    Debug.Log("aktueller Score:" + calculatedScore + " von : " + availableActions[i]);

                    if (calculatedScore >= score)
                    {
                        bestActionIndex = i;
                        score = availableActions[i].Score;


                    }
                }

                if (availableActions[bestActionIndex].Score == 0.0f)
                {
                    // Debug.Log("Alle Scores sind 0 ");
                    bBestActionFound = false;
                    return;
                }

                bestAction = availableActions[bestActionIndex];
                Debug.Log($"UtilityAI:: beste Aktion für {this.npcAiController} ist: {this.bestAction.name}");

                bBestActionFound = true;
                bFinishedDeciding = true;


            }
        }
        public float ScoreAction(Action action)
        {

            float score = 0.0f;
            int uNumConsiderations = action.considerations.Length;
            if (uNumConsiderations > 0)
            {
                uint uNumDoNotAffectScore = 0;

                for (int i = 0; i < uNumConsiderations; i++)
                {
                    if (action.considerations[i].bAffectScore_ == false)
                        uNumDoNotAffectScore++;
                    //TODO: abhängige Considerations bei einer consideration (für was nochmal? bruachen wir das sptäter?
                    //if (action.considerations[i].dependentConsiderations.Length > 0)
                    //{
                    //    for (int j = 0; j < action.considerations[i].dependentConsiderations.Length; j++)
                    //        action.considerations[i].dependentConsiderations[j].ScoreConsideration();
                    //}

                    float considerationScore = action.considerations[i].ScoreConsideration(npcAiController);
                    Debug.Log("aktueller Score:" + considerationScore + " von : " + action + " Consideration: " + action.considerations[i]);


                    //wenn eine Consideration einer aktion null hat, sollte die aktion garnicht erst ausgeführt werden
                    if (considerationScore == 0)
                    {
                        action.Score = 0;
                        return action.Score;
                    }
                    else if (i == 0 && action.considerations[i].bAffectScore_)
                        score = considerationScore;
                    else if (i > 0 && action.considerations[i].bAffectScore_)
                        score += considerationScore;
                }
                //TODO: Dave Marks averagivng Scheme implementieren evtl! lecutres nochmal gucken
                action.Score = (score / (uNumConsiderations - uNumDoNotAffectScore));
            }


            return action.Score;


        }
        public Action getBestAction()
        {
            return bestAction;
        }


    }
}