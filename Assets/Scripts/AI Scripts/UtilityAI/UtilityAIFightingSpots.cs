using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UtilityAI
{
    public class UtilityAIFightingSpots : MonoBehaviour
    {
        //public List<Action> actions = new List<Action>();
        public List<Action> bestActions;
        public Action[] sortedActions;
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
            float calculatedScore = -1.0f;
            float maxScore = -1;

            if (availableActions.Length > 0)
            {
                for (int i = 0; i < availableActions.Length; i++)
                {
                    calculatedScore = ScoreAction(availableActions[i]);
                    //Debug.Log("aktueller Aktion: " + availableActions[i].name + "Score: " + calculatedScore);

                  
                    if (calculatedScore > 0)
                    {
                        maxScore = calculatedScore;
                        bestActions.Add(availableActions[i]);
                    }
                }

                sortedActions = bestActions.OrderBy(a => a.Score).ToArray();
                //Debug.Log("Beste Aktion(en):");
                foreach (Action action in sortedActions)
                {
                    //Debug.Log("Aktion: " + action.name + " Score: " + action.Score.ToString());
                }

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


    }
}