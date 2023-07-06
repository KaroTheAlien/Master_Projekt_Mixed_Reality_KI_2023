using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI_Scripts;
using UnityEngine;
using UnityEngine.AI;
using UtilityAI;
using static AI_Scripts.EnemySfx.Type;
using Action = UtilityAI.Action;

public class Enemy : EnemyParent
{
    [HideInInspector] public EnemySfx enemySfx;
    Animator enemyAnimator;
    public int currPostureDmg = 0;
    [SerializeField] int minPostureDmg = 1;
    [SerializeField] int midPostureDmg = 10;
    [SerializeField] int maxPostureDmg = 20;
    [SerializeField] GameObject hpBarProgress;
    //string[] names = { "Attack_Right", "Attack_DownLeft", "Attack_DownRight", "Attack_Left", "Attack_LeftUp", "Attack_RightUp", "Attack_Up_RightHand" };
    AttackDirection currentAttack;
    float attackSpeed = 5;
    //animation Times: 
    float timeAttackRight = 0;
    float timeAttackDownRight = 0;
    float timeAttackRightUp = 0;
    float timeAttackUp_RightHand = 0;
    float timeAttackLeft = 0;
    float timeAttackDownLeft = 0;
    float timeAttackLeftUp = 0;

    [SerializeField]  Material dissolveMaterial;
    Material dissolveMaterialInstance;
    bool dissolve=false;
    public UtilityAI.UtilityAI UtilityaiReasoner { get; set; }
    public UtilityAIDirections UtilityaiReasonerDirections { get; set; } //quasi copy paste für 2. Instanz für richtungen "seperat"
    public UtilityAIFightingSpots UtilityaiReasonerFightingSpots { get; set; } //quasi copy paste für 2. Instanz für richtungen "seperat"
    private Action[] sortedFightingSpotsByScore;
    private List<FightSpotPosition> scoredPositionPool = new();


    public Action[] actions;
    public Action[] attackDirections;
    public Action[] fightingSpots; //spots als aktionen

    //für das Wissen des NPCs
    [HideInInspector]
    public GameObject player; // für Wissen über Spieler // maybe später über sensoren -> in sicht // get methode für zugriff statt public
    [HideInInspector]
    public List<GameObject> PlayerSwords;
    [HideInInspector]
    public List<GameObject> Colliders;

    [HideInInspector]
    public FightSpotController FightSpotController;
    [HideInInspector]
    public GameObject CurrentTargetSpot;
    public NavMeshAgent navAgent;

    private bool bAiCalcIsRunning = false;
    private bool bExecutingAction = false;
    private bool bDoingFastAttack = false;
    private bool bDoingHeavyAttack = false;
    private bool hasMoved = false;
    private bool callStopMoving = false;

    int currAttack = 0;
    
    public bool bFightingSpotsCalulated = false;
    public bool bAlreadyClaimedSpot = false;

    private float rotationSpeed = 15f; // Geschwindigkeit der Rotation
    private bool bOnSpot = false;
    private bool bDoneRotating = false;

    public bool bBlockedTarget = false;

    [HideInInspector]
    public float attackRange = 0.0f;
    private EnemyGroupBehaviourController groupBehaviourController;

    public bool DebugParry = false;

    public bool bBLOCKER = false;
    private void Start()
    {
        groupBehaviourController = GameObject.FindGameObjectWithTag("GroupBehaviourController").GetComponent<EnemyGroupBehaviourController>();
        FightSpotController = FindObjectOfType<FightSpotController>();
        if (bBLOCKER)
            return;
        //setup stuff
        this.enemySfx = this.GetComponent<EnemySfx>();
        player = GameObject.FindGameObjectWithTag("Player");
        GetGOwithTag(player.transform, "PlayerSwordTip", PlayerSwords);
        GetGOwithTag(this.transform, "Collider", Colliders);
        enemyAnimator = GetComponentInChildren<Animator>();
        UtilityaiReasoner = GetComponent<UtilityAI.UtilityAI>();
        UtilityaiReasonerDirections = GetComponent<UtilityAIDirections>();
        UtilityaiReasonerFightingSpots = GetComponent<UtilityAIFightingSpots>();
        enemyAnimator.SetFloat("AnimationSpeedScale", attackSpeed);

        navAgent = GetComponent<NavMeshAgent>();
        //navAgent.autoTraverseOffMeshLink = false; //link
        //nur zu BEGINN damit kein error geschmissen wird
        CurrentTargetSpot = player;
        
        attackRange = Mathf.Abs(Colliders[0].transform.position.z - this.transform.position.z);
        Debug.Log("attack Range: " + attackRange.ToString());
        dissolveMaterialInstance = new Material( dissolveMaterial);
        this.enemySfx.PlaySound(AGGRO);
    }
    
    public float GetAttackSpeed()
    {
        return attackSpeed;
    }
    public void SetAttackSpeed(float speed)
    {
        attackSpeed = speed;
    }

    private void Update()
    {
        if (bBLOCKER)
            return;
        if (DebugParry == true)
        {
            DebugParry = false;
            ChangePosture(200);
        }
        if (dissolve)
        {
            dissolveMaterialInstance.SetFloat("_DissolveAmount", dissolveMaterialInstance.GetFloat("_DissolveAmount") + Time.deltaTime);
            if (dissolveMaterialInstance.GetFloat("_DissolveAmount") > 1.5f)
            {
                dissolve = false;
                this.gameObject.SetActive(false);
                this.FightSpotController.ReleaseSpot(this);
                Destroy(this.gameObject);
                groupBehaviourController.ClearAttack();
            }
        }
        if(!bDoneRotating &&(navAgent.isStopped || bOnSpot))
        {
            // Berechnung der Drehung zum Spieler hin
            Vector3 directionToPlayer = player.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);

            if (this.gameObject.name.Equals("Boss(Clone)") || this.gameObject.name.Equals("Boss"))
            {
                //threshold = 5f;
                //transform.rotation = targetRotation;
                bDoneRotating = true;
            }
            else
            {
                // Sanfte Interpolation der Rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                if (!bDoneRotating && Quaternion.Angle(transform.rotation, targetRotation) <= 0.1f)
                {
                    transform.rotation = targetRotation;
                    bDoneRotating = true;
                }
            }

        }
        if (this.gameObject.name.Equals("Boss(Clone)") || this.gameObject.name.Equals("Boss"))
        {
            // Berechnung der Drehung zum Spieler hin
            Vector3 directionToPlayer = player.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            transform.rotation = targetRotation;
        }
        //1. Start in update weil sonst alles andere noch ncih initialisiert oder so
        if (UtilityaiReasoner.bBestActionFound == false)
        {
            UtilityaiReasoner.CalculateBestAction(actions);
            if (UtilityaiReasoner.bBestActionFound == false && this.hasMoved)
            {
                this.hasMoved = false;
                this.StopMoving();
            }
        }

        //directions
        if (UtilityaiReasonerDirections.bBestActionFound == true)
        {
            // Debug.Log("Direction found!");
            UtilityaiReasonerDirections.bBestActionFound = false;
            this.callStopMoving = true;
            UtilityaiReasonerDirections.bestAction.Execute(this);
            if (this.callStopMoving && this.hasMoved)
            {
                this.hasMoved = false;
                this.StopMoving();
            }
        }
        //fightingSpots
        if(UtilityaiReasonerFightingSpots.bBestActionFound == true) 
        {
            // Debug.Log("Fighting Spots found!");
            UtilityaiReasonerFightingSpots.bBestActionFound = false;
            sortedFightingSpotsByScore = UtilityaiReasonerFightingSpots.sortedActions;
        }
        //nur dann setzen wenn bereits die verfügbaren Spots bewertet worden und noch kein Spot geclaimed wurde 
        if (bFightingSpotsCalulated == true && bAlreadyClaimedSpot == false)
        {
            setNearestAvailableSpotAsTarget();
        }


        //beste Aktion ausführen
        if (UtilityaiReasoner.bBestActionFound && this.bExecutingAction == false)
        {
            // UtilityaiReasoner.bBestActionFound = false;
            this.bExecutingAction = true;
            UtilityaiReasoner.bestAction.Execute(this);
        }
        //Angriff ausführen nachdem die richtung bestimmt wurde
        else if (this.bDoingFastAttack && this.bDoneRotating && groupBehaviourController.RequestAttack(this))
        {
            this.bDoingFastAttack = false;
            this.StartCoroutine(this.AttackFastCoroutine());
        }
        else if (this.bDoingHeavyAttack && this.bDoneRotating && groupBehaviourController.RequestAttack(this))
        {
            this.bDoingHeavyAttack = false;
            // this.enemyAttackSounds.Random()?.PlayDelayed(0.25f);
            // this.weaponAttackSounds.Random()?.PlayDelayed(1f);
            this.StartCoroutine(this.AttackHeavyCoroutine());  
        }

        //falls man nur animation vom dude abspielen will:     
        //currAttack++;
        //if (currAttack > 6)
        //{
        //    currAttack = 0;
        //}
        //enemyAnimator.SetBool(names[currAttack], true);
        //if (currAttack != 0) {
        //    enemyAnimator.SetBool(names[currAttack - 1], false);
        //}
        //else
        //{
        //    enemyAnimator.SetBool(names[6], false);
        //}
        //if (bBlockedTarget)
        //    navAgent.speed = 0;
        //else
        //    navAgent.speed = 1;
    }

    // player successfully parried the enemy attack //von EnemyEventHandlerIguess -> hier noch anpassungen machen
    public override void Parried()
    {
        enemyAnimator.SetBool("Parried", true);
        StartCoroutine(WaitForResetParried(0.25f));

    }
    public override void PauseAnimation()
    {
        enemyAnimator.speed = 0;
    }

    // add posture damage to the enemy
    public void ChangePosture(int postureChange)
    {
        currPostureDmg += postureChange;
        float hpBarScale = (float)currPostureDmg / 100.0f;
        hpBarProgress.transform.localScale = new Vector3(1, hpBarScale, 1);
        // Debug.Log("cure posture" + currPostureDmg);
        if (currPostureDmg >= 100)
        {
            this.enemySfx.PlaySound(DEFEAT);
            enemyAnimator.SetBool("Defeat", true);
            SwitchMaterialsDissolve();
        }
    }

    public int getPosture()
    {
        return currPostureDmg;
    }
    
    public override void  CalculatePostureDamage(float distance)
    {
        Debug.Log("Distance + " + distance);
        if (distance < 0.6f)
        {
            ChangePosture(minPostureDmg);
        }
        else if (distance < 1.0f)
        {
            ChangePosture(midPostureDmg);
        }
        else
        {
            ChangePosture(maxPostureDmg);
        }
    }

    void SwitchMaterialsDissolve()
    {
        Renderer[] sRender;
        sRender = GetComponentsInChildren<Renderer>();
        foreach(Renderer currRend in sRender)
        {

            Material[] test = new Material[currRend.materials.Length];
            for (int i = 0; i < test.Length; i++)
            {
                test[i] = dissolveMaterialInstance;
            }
            currRend.materials = test;
          
        }
        dissolve = true;
       
    }

    //alle angriffszeiten berechnenn //maybe noch so ädnern dass nur die zeit geupdated wird die als näcstes gebraucht wird
    public void UpdateAnimClipTimes()
    {
        AnimationClip[] clips = enemyAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            Debug.Log(clip.length + " clip länge");
            switch (clip.name)
            {
                case "Armature.001|Attack_Right":
                    timeAttackRight = clip.length;
                    break;
                case "Armature.001|Attack_DownRight":
                    timeAttackDownRight = clip.length ;
                    break;
                case "Armature.001|Attack_RightUp":
                    timeAttackRightUp = clip.length;
                    break;
                case "Armature.001|Attack_Up RightHand":
                    timeAttackUp_RightHand = clip.length ;
                    break;
                case "Armature.001|Attack_Left":
                    timeAttackLeft = clip.length ;
                    break;
                case "Armature.001|Attack_DownLeft":
                    timeAttackDownLeft = clip.length ;
                    break;
                case "Armature.001|Attack_LeftUp":
                    timeAttackLeftUp = clip.length;
                    break;
            }
        }
    }
    
    //Gibt die Zeit zurück die die aktuelle Animation des Angriffs benötigt!
    public float GetTimeForCurrentAttack()
    {
        return this.currentAttack switch
        {
            AttackDirection.Right => this.timeAttackRight,
            AttackDirection.DownRight => this.timeAttackDownRight,
            AttackDirection.RightUp => this.timeAttackRightUp,
            AttackDirection.UpRightHand => this.timeAttackUp_RightHand,
            AttackDirection.Left => this.timeAttackLeft,
            AttackDirection.DownLeft => this.timeAttackDownLeft,
            AttackDirection.LeftUp => this.timeAttackLeftUp,
            _ => throw new ArgumentOutOfRangeException(nameof(this.currentAttack), this.currentAttack, null)
        };
    }

    //Gameobject parent mit tag tag in liste list hinzufügen
    void GetGOwithTag(Transform parent, string tag, List<GameObject> list)
    {

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == tag)
            {
                list.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                GetGOwithTag(child, tag, list);
            }
        }
    }

    public bool getIfAlreadyClaimedSpot()
    {
        return bAlreadyClaimedSpot;
    }

    //Aktion ruft auf welche art von Attacke am besten sein wird ( bisher nur normal - also fast)
    public void AttackFast()
    {
        // Debug.Log("Attack Fast");

        UtilityaiReasonerDirections.CalculateBestAction(attackDirections);
        bDoingFastAttack = true;
    }
    public void AttackHeavy()
    {
        UtilityaiReasonerDirections.CalculateBestAction(attackDirections);
        bDoingHeavyAttack = true;
    }

    //wird von einer aktion aufgerufen welche Richtung als nächstes am besten ist.
    public void Attack(AttackDirection direction)
    {
        this.currentAttack = direction;
    }

    public void AddSpotToPool(FightSpotPosition position)
    {
        this.scoredPositionPool.Add(position);
    }

    //von Aktion aufgerufen!
    public void ClaimNearestAvailableSpot() 
    {
        UtilityaiReasonerFightingSpots.CalculateBestAction(fightingSpots);

        bFightingSpotsCalulated = true;
    }

    //von update gecalled
    public void setNearestAvailableSpotAsTarget()
    {
        this.scoredPositionPool.Clear();
        // add spots to pool, reversed to add best scored spots first
        foreach (Action action in this.sortedFightingSpotsByScore.Reverse())
        {
            action.Execute(this);
        }

        if (this.scoredPositionPool.Count != 3)
        { // fallback, if fight spots were not found.
            this.scoredPositionPool = Enum.GetValues(typeof(FightSpotPosition)).Cast<FightSpotPosition>().ToList();
        }
        
        GameObject result = this.FightSpotController.ClaimSpot(this, this.scoredPositionPool);
        this.CurrentTargetSpot = result;
        if (result is null)
        {
            Debug.Log("not enough spots / too many enemies");
            // TODO execute AfterFinishedAction even if no spot was assigned? => review later
        } else
        {
            this.bAlreadyClaimedSpot = true;
            this.AfterFinishedAction();
        }
    }
    
    public void MoveToPlayerSweetSpot()
    {
        //Todo walk towards player position.
        //TODO: NavMesh -> zum spieler bewegen! neue coroutine? 
        //maybe auch nur einen step zum spieler machen statt ganze utility ai berechnung warten zu lassen
        //anderererseits gibt es keine aktion die wichtiger sein sollte auf dem weg zum spieler (nichts ausweichen usw)
        //Debug.Log("going to move to FightingSpot " + CurrentTargetSpot.name);
        this.callStopMoving = false;
        this.hasMoved = true;
        this.bOnSpot = false;
        navAgent.isStopped = false;
        if(CurrentTargetSpot!= null)
        {
            navAgent.destination = CurrentTargetSpot.transform.position;

        }
        else
        {
            navAgent.isStopped = true;
        }
        if (navAgent.speed > 0.01) enemyAnimator.SetBool("Walk", true);
        AfterFinishedAction();
    }
   
    public void StopMoving()
    {
        navAgent.isStopped = true;
        bOnSpot = true;
        bDoneRotating = false;
        Debug.Log("Stopped Movmenet");
        enemyAnimator.SetBool("Walk", false);
        AfterFinishedAction();
    }

    public void StopMoving2()
    {
        navAgent.isStopped = true;
        bOnSpot = true;
        bDoneRotating = false;
        Debug.Log("Stopped Movmenet");
        enemyAnimator.SetBool("Walk", false);
    }
    //nach abschluss einer aktion die nächste berechnung starten!
    public void AfterFinishedAction()
    {
        this.bExecutingAction = false;
        UtilityaiReasoner.bBestActionFound = false;
        // UtilityaiReasoner.CalculateBestAction(actions);
    }


    //Coroutines die Zeit brauchen
    #region Coroutine

    // wait for reseting the parried variable in the animator 
    IEnumerator WaitForResetParried(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        enemyAnimator.SetBool("Parried", false);
    }
    //Attack Fast
    IEnumerator AttackFastCoroutine()
    {
        // bDoingFastAttack = false;
        //Debug.Log("Attacking fast Courotine");
        if (this.gameObject.name.Equals("Boss(Clone)") || this.gameObject.name.Equals("Boss"))
        {
            // Berechnung der Drehung zum Spieler hin
            Vector3 directionToPlayer = player.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            transform.rotation = targetRotation;
            attackSpeed = 0.85f;
        }
        else
        {
            attackSpeed = 5f;
        }
        enemyAnimator.SetFloat("AnimationSpeedScale", attackSpeed);
        UpdateAnimClipTimes();

        //Debug.Log("Current Fast Animation Time " + GetTimeForCurrentAttack());
        this.enemyAnimator.SetBool(this.currentAttack.Name(), true);
        //Debug.Log(GetTimeForCurrentAttack() + " testabc" + currentAttack);
        yield return new WaitForSeconds(0.2f + GetTimeForCurrentAttack() / attackSpeed);
        this.enemyAnimator.SetBool(this.currentAttack.Name(), false);

        groupBehaviourController.ClearAttack();
        AfterFinishedAction();
    }
    //attack Heavy noch nicht implementiert - maybe auch weglassen ehrer
    IEnumerator AttackHeavyCoroutine()
    {
        Debug.Log("Attacking Heavy");
        attackSpeed = 1;
        enemyAnimator.SetFloat("AnimationSpeedScale", attackSpeed);
        UpdateAnimClipTimes();

        Debug.Log("Current Heavy Animation Time " + GetTimeForCurrentAttack());
        this.enemyAnimator.SetBool(this.currentAttack.Name(), true);
        yield return new WaitForSeconds(GetTimeForCurrentAttack());
        this.enemyAnimator.SetBool(this.currentAttack.Name(), false);

        groupBehaviourController.ClearAttack();
        AfterFinishedAction();
    }

    IEnumerator StartDissolveEffect()
    {
        float curr = 0.5f;
       while (curr < 1.5f)
        {
            curr += 0.0001f;
            dissolveMaterialInstance.SetFloat("_DissolveAmount", curr);
       
        }
        yield return null;
    }

    #endregion

}
