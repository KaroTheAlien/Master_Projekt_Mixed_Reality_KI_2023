using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Consideration : ScriptableObject
{
    public string Name;
    public string description;
    //maybe
    public Consideration[] dependentConsiderations;
    
    public bool bAffectScore;
    private float privateScore;

    //TODO: Kurvenzeug


    public bool bAffectScore_
    {
        get { return bAffectScore; }
        set { bAffectScore = value; }
    }
    public float Score

    {
        get { return privateScore; }
        set { privateScore = Mathf.Clamp01(value); } // zwischen 0 und 1 maximal
    }


    public virtual void Awake()
    {
        Score = 0;
    }

    //public abstract float ScoreConsideration(NPCAIController npcAiController);
    public abstract float ScoreConsideration(Enemy npcAiController);


}
