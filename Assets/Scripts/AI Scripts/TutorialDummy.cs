using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDummy : EnemyParent
{
    int currAnimationIndex = 0;
    [SerializeField] Animator enemyAnimator;
    string lastAnimName = "Attack_Left";
   

    private void Start()
    {
        enemyAnimator.SetBool("Attack_Left", true);
    }
    public override void Parried()
    {
        enemyAnimator.SetBool("Parried", true);
        StartCoroutine(WaitForResetParried(0.25f));
    }
    public override void PauseAnimation()
    {
        enemyAnimator.speed = 0;
    }
    IEnumerator WaitForResetParried(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        enemyAnimator.SetBool("Parried", false);
    }

    public override void CalculatePostureDamage(float highestDistance)
    {
        enemyAnimator.speed = 0.5f;
        nextAttack();
        return;
    }

    private void nextAttack()
    {
        enemyAnimator.SetBool(lastAnimName, false);
        currAnimationIndex += 1;
        currAnimationIndex = currAnimationIndex % 4;
        switch (currAnimationIndex)
        {
            case 0:
                lastAnimName = "Attack_Left";
                enemyAnimator.SetBool(lastAnimName, true);
                return;
            case 1:
                lastAnimName = "Attack_LeftUp";
                enemyAnimator.SetBool(lastAnimName, true);
                return;
            case 2:
                lastAnimName = "Attack_Right";
                enemyAnimator.SetBool(lastAnimName, true);
                return;
            case 3:
                lastAnimName = "Attack_RightUp";
                enemyAnimator.SetBool(lastAnimName, true);
                return;

        }
    }
    // Update is called once per frame



}
