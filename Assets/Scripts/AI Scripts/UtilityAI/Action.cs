using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UtilityAI
{
    public abstract class Action : ScriptableObject
    {
        //Für den jeweiligen NPC:
        // public NPCAIController npc;
        public string Name;
        public string description;
        public Consideration[] considerations;

        private float privateScore;
        private float privateCooldown; //maybe
                                       //public get/set
        public float Score
        {
            get { return privateScore; }
            set { privateScore = Mathf.Clamp01(value); } // zwischen 0 und 1 maximal
        }
        public float Cooldown
        {
            get { return privateCooldown; }
            set { privateCooldown = value; } // zwischen 0 und 1 maximal
        }


        public virtual void Awake()
        {
            Score = 0;
        }

        //public abstract void Execute(NPCAIController npc);
        public abstract void Execute(Enemy npc);



    }
}