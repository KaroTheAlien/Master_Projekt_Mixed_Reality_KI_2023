using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryObject : MonoBehaviour
{
    EnemyParent enemyParent;
    [SerializeField] ParryObject connectedCollider;
    [SerializeField] ParticleSystem parryParticleSystem;
    bool enemySwordCollides = false;
    bool playerSwordCollides = false;
   EnemyEventHandler enemyEventHandler;
    Haptics currHaptics = null;
    private void Start()
    {enemyParent = GetComponentInParent<EnemyParent>();
        enemyEventHandler = enemyParent.GetComponent<EnemyEventHandler>();
        
    }

    //check if both swords are or were in the collider in between a timespan. if yes => parried
    private void Update()
    {
        if(enemySwordCollides && playerSwordCollides)
        {
            resetValues();
            connectedCollider.resetValues();
            enemyParent.Parried();
            enemyEventHandler.DeactivateColliders(true);
            currHaptics.Vibrate(0.125f, 220, 0.75f);
            parryParticleSystem.transform.position = this.transform.position;
            parryParticleSystem.Play();

        }
    }
    // reset both checks for sword collision (has to be done on both colliders which are linked up to the attack)
    public void resetValues()
    {
        enemySwordCollides = false;
        playerSwordCollides = false;
    }


    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.tag.Equals("PlayerSword"))
        {
            playerSwordCollides = true;
            currHaptics = other.transform.parent.GetComponent<Haptics>();
        }
        if (other.gameObject.tag.Equals("EnemySword"))
        {    
            enemySwordCollides = true;
            if (!playerSwordCollides)
            {
                SwitchArrows(true);
                
            }
        }
        if (other.gameObject.tag.Equals("TutorialSword"))
        {
            enemyParent.PauseAnimation();
            enemySwordCollides = true;
            if (!playerSwordCollides)
            {
                SwitchArrows(true);
            }
        }
    }

    public void SwitchArrows(bool nowPariable)
    {
        if(this.transform.childCount == 0)
        {
            connectedCollider.SwitchArrows(nowPariable);
        }
        else if (nowPariable == true)
        {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.Find("Arrow Red").gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.Find("Arrow Red").gameObject.SetActive(false);
        }
    }
    // after one sword is leaving the collider wait for an amount of time before reseting the bool (balancing etc.)
    private void OnTriggerExit(Collider other)
    {
        string colTag = other.gameObject.tag;
        if (colTag.Equals("PlayerSword") || colTag.Equals("EnemySword"))
        {
            StartCoroutine(WaitForDeactivation(0.1f,colTag));
        }
    }

    IEnumerator WaitForDeactivation(float time, string swordTag)
    {
        yield return new WaitForSecondsRealtime(time);
       
        switch (swordTag)
        {
            case "PlayerSword":
                playerSwordCollides = false;
                break;
            case "EnemySword":
                if (this.transform.childCount == 0)
                {
                    break;
                }
                enemySwordCollides = false;
                enemyEventHandler.DeactivateColliders(false);
                break;
        }
    }

    IEnumerator WaitForPlayerHitDelay(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        playerSwordCollides = false;
    }
}
