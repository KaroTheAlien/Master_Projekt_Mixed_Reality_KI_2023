using System.Collections;
using AI_Scripts;
using UnityEngine;
using static AI_Scripts.EnemySfx.Type;

public class EnemyEventHandler : MonoBehaviour
{
    [SerializeField] Animator enemyAnimator;
    [SerializeField] GameObject[] Colliders;
    EnemyParent enemyObject;
    GameObject currCollider;
    [SerializeField]  float timeForColliderDeactivation = 0.2f;
    [SerializeField] int damage = 10;
    [SerializeField] Player playerObj;
    [SerializeField] Transform playerSwordTipTransform;
    Vector3 swordTipStartPos;
    float highestDistance;
    bool calculateParryDamage;
    int frameCheck = 5;
    bool parried = false;
    private void Start()
    {
        currCollider = Colliders[0];
        enemyObject = this.GetComponent<EnemyParent>();
        playerSwordTipTransform = GameObject.FindGameObjectWithTag("PlayerSwordTip").transform;
        playerObj = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>(); // fur spawned

    }
    private void Update()
    {
        if (calculateParryDamage)
        {
            frameCheck -= 1;
        }
    }

    public void OnEnemySoundEvent(string msg)
    {
        EnemySfx sfx = this.GetComponent<Enemy>().enemySfx;
        switch (msg)
        {
            case "attack":
                sfx.PlaySound(ENEMY_ATTACK);
                sfx.PlaySound(WEAPON_ATTACK, 0.75f);
                break;
            case "parry":
                sfx.PlaySound(WEAPON_PARRIED);
                sfx.PlaySound(ENEMY_PARRIED, 0.1f);
                break;
        }
    }

    // activates the collider linked to the current attack. setup in the animation as event. msg = the name of the collider game object
    public void OnEnemyAnimationEvent(string msg)
    {
        foreach(GameObject curr in Colliders)
        {
            if (curr.name.Equals(msg))
            {
                if (currCollider.activeInHierarchy)
                {
                    currCollider.SetActive(false);
                }
                currCollider = curr;
                curr.transform.gameObject.SetActive(true);
                curr.GetComponent<ParryObject>().SwitchArrows(false);
                swordTipStartPos = playerSwordTipTransform.position;
                highestDistance = Vector3.Distance(swordTipStartPos, currCollider.transform.position);
                StartCoroutine(CalculateParryDistance());
            }
        }
    }
    // deactivate the currently active collider. deactivate immediately when parried
    public void DeactivateColliders(bool deactivateImmediately)
    {
        if (!deactivateImmediately)
        {
            StartCoroutine(WaitForArrowDeactivate(timeForColliderDeactivation));
        }
        else
        {
            currCollider.SetActive(false);
            parried = true;
            enemyObject.CalculatePostureDamage(highestDistance);
        }
    }
    // deactivate the collider after a certain amount of time (balancing etc.)
    // deal damage to the player
    IEnumerator WaitForArrowDeactivate(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        currCollider.SetActive(false);
        playerObj.TakeDamage(damage);
    }

    IEnumerator CalculateParryDistance()
    {
        parried = false;
        //Debug.Log("highestDistanceinit" + highestDistance);
        yield return new WaitWhile(() => frameCheck > 0);
        float currDistance = Vector3.Distance(playerSwordTipTransform.position, currCollider.transform.position);
        if (currDistance > highestDistance)
        {
            highestDistance = currDistance;
        }
        if(parried == false)
        {
            frameCheck = 5;
            StartCoroutine(CalculateParryDistance());
        }
    }
}
