using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class DragonAttack : MonoBehaviour
{
    [Header("Attackrange")]
    public float triggerDistance = 4.0f;
    [Header("List of all enemies")]
    public List<GameObject> EnemyList;
    [Header("Current Target Enemy")]
    public GameObject Enemy;

    private GameObject player;
    private int playerHealth;
    private int threshold = 30;
    private int dmgPerSec = 1;
    private float attackTimer = 0f;
    private float attackSpeed;

    [Header("Damaging Effect")]
    [SerializeField] public GameObject dmgBaseParticles;
    [SerializeField] public GameObject dmgParticles;
    [Header("Slowing Effect")]
    [SerializeField] public GameObject slowBaseParticles;
    [SerializeField] public GameObject slowParticles;

    GameObject damaging;
    GameObject slowing;

    DragonStateController dragonStateController;
    DragonBehaviourController dragonBehaviourController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        dragonStateController = GetComponent<DragonStateController>();
        dragonBehaviourController = GetComponent<DragonBehaviourController>();

        // Add all enemies in the Scene in the list
        EnemyList = new List<GameObject>();
        UpdateEnemyList();
    }

    void Update()
    {
        CheckOnPlayerHP();

        UpdateEnemyList();

        FindNearestEnemy();
    }

    void FindNearestEnemy()
    {
        // Check every opponent in the list
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        int attackingEnemiesCount = 0;
        int initialCount = 0;

        foreach (GameObject enemy in EnemyList)
        {
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);

            // if the enemy is in attackrange
            if (distance <= triggerDistance)
            {
                attackingEnemiesCount++;

                if (distance < nearestDistance)
                {
                    nearestEnemy = enemy;
                    nearestDistance = distance;
                }

                // if the Player is attacked by 3 or more enemies, Dragon will start to Distract an Enemy
                //if (attackingEnemiesCount >= 3)
                //{
                //    dragonStateController.SetAttackState(DragonStateController.States.Distract);
                //    initialCount = EnemyList.Count;
                //}

                // if an Enemy dies, end distracting
                if (dragonStateController.currentAttackState == DragonStateController.States.Distract && EnemyList.Count < initialCount)
                {
                    dragonStateController.SetAttackState(DragonStateController.States.Damage);
                }
                
            }
        }

        // If no enemy is found, end Attackmode
        if (nearestEnemy == null)
        {
            Enemy = null;
            dragonStateController.SetState(DragonStateController.States.Support);
            if (damaging || slowing)
            {
                Destroy(damaging);
                Destroy(slowing);
            }
            return;
        }

        // If the enemy is in Attackrange switch to Attackmode
        if (nearestDistance <= triggerDistance)
        {
            Enemy = nearestEnemy;
            dragonStateController.SetState(DragonStateController.States.Attack);
            return;
        }
    }

    void UpdateEnemyList()
    {
        // Remove inactive enemies from the list
        EnemyList.RemoveAll(enemy => enemy == null || !enemy.activeSelf);

        // Add all enemies in the scene to the list
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            // Only add the enemy to the list, if he isnt in the list already
            if (!EnemyList.Contains(enemy))
            {
                EnemyList.Add(enemy);
            }
        }
    }

    //Switch Target to nearest Enemy
    public void AttackEnemy()
    {
        dragonBehaviourController.SetTarget(Enemy.transform.Find("DragonTarget"));
    }

    private void CheckOnPlayerHP()
    {
        playerHealth = player.GetComponent<Player>().GetHP();

        // switch between Attackmodes if playerhealth is low
        if (playerHealth <= threshold && dragonStateController.currentBaseState == DragonStateController.States.Attack)
        {
            dragonStateController.SetAttackState(DragonStateController.States.Slow);
            Destroy(damaging);
            attackSpeed = 3f;
        }
        else
        {
            dragonStateController.SetAttackState(DragonStateController.States.Damage);
            Destroy(slowing);
            attackSpeed = 5f;
        }
    }

    // Increase posturebar of enemy
    public void DamageEnemy()
    {
        if (slowing)
        {
            Destroy(slowing);
        }
        if (!damaging)
        {
            damaging = Instantiate(dmgBaseParticles, new Vector3(Enemy.transform.position.x, 0.1f, Enemy.transform.position.z), Quaternion.identity, Enemy.transform);
        }

        //Increase Posturebar of enemy every second
        attackTimer += Time.deltaTime;
        if (attackTimer >= 1f)
        {
            attackTimer = 0f;
            Enemy.GetComponent<Enemy>().ChangePosture(dmgPerSec);
        }
    }

    // decrease attackspeed of enemy
    public void SlowEnemy()
    {
        if (damaging)
        {
            Destroy(damaging);
        }
        if (!slowing)
        {
            slowing = Instantiate(slowBaseParticles, new Vector3(Enemy.transform.position.x, 0.1f, Enemy.transform.position.z), Quaternion.identity, Enemy.transform);
        }

        Enemy.GetComponent<Enemy>().SetAttackSpeed(attackSpeed);
    }
}
