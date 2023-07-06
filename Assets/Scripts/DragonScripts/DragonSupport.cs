using System.Collections;
using UnityEngine;

public class DragonSupport : MonoBehaviour
{
    [Header("Player Target")]
    [SerializeField] private GameObject playerCam;
    private GameObject player;
    private int playerHealth;
    private float healingRate = 5f;
    private int healingThreshold = 30;
    private int healingAmount = 50;
    private float cooldownTime = 10f;
    [Header("Healing Effect")]
    [SerializeField] public GameObject healParticles;
    [SerializeField] public GameObject healBasePlayer;
    GameObject healing;

    private bool isHealing = false;
    private float lastHealTime = 0f;
    private float cooldownEndTime = 0f;

    DragonStateController dragonStateController;
    DragonBehaviourController dragonBehaviourController;
    Player healPlayer;

    void Start()
    {
        dragonStateController = GetComponent<DragonStateController>();
        dragonBehaviourController = GetComponent<DragonBehaviourController>();
        player = GameObject.FindGameObjectWithTag("Player");
        healPlayer = player.GetComponent<Player>();
    }

    private void Update()
    {
        CheckPlayerHP();

        if (isHealing && Time.time - lastHealTime >= healingAmount / healingRate)
        {
            isHealing = false;
            cooldownEndTime = Time.time + cooldownTime;
        }
    }

    // Change the target of the steering behaviors to the player
    public void FollowPlayer()
    {
        dragonBehaviourController.SetTarget(playerCam.transform);
    }

    // Heal player if he is under 30 HP and out of fighting
    public IEnumerator HealPlayer()
    {
        healing = Instantiate(healBasePlayer, new Vector3(playerCam.transform.position.x, 0.2f, playerCam.transform.position.z), Quaternion.identity, playerCam.transform);
        isHealing = true;
        lastHealTime = Time.time;

        float timeBetweenIncrements = 0.2f; // +1 HP every 0,2 seconds
        int remainingHealing = healingAmount;

        while (remainingHealing > 0)
        {
            playerHealth = Mathf.Min(playerHealth + 1, 100);
            healPlayer.SetHP(playerHealth);
            remainingHealing--;
            if (remainingHealing > 0)
            {
                // Waitingtime
                yield return new WaitForSeconds(timeBetweenIncrements);
            }
        }

        Destroy(healing);
        isHealing = false;
        cooldownEndTime = Time.time + cooldownTime;
        dragonStateController.SetSupportState(DragonStateController.States.Follow);
    }


    // if player is below 30 HP switch to heal state
    void CheckPlayerHP()
    {
        playerHealth = healPlayer.GetHP();

        // Check if Player should be healed
        if (playerHealth <= healingThreshold && !isHealing && Time.time > cooldownEndTime && dragonStateController.currentBaseState == DragonStateController.States.Support)
        {
            // Start Healing
            dragonStateController.SetSupportState(DragonStateController.States.Heal);
            StartCoroutine(HealPlayer());
        }
    }
    
}
