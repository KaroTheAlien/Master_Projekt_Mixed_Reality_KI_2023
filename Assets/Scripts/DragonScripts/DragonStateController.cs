using System.Collections.Generic;
using UnityEngine;

public class DragonStateController : MonoBehaviour
{
    DragonAttack dragonAttack;
    DragonSupport dragonSupport;
    DragonGuide dragonGuide;
    DragonBehaviourController dragonBehaviourController;

    // all Dragon states
    public enum States
    {
        Support,
        Attack,
        Damage,
        Slow,
        Distract,
        Heal,
        Follow,
        Guide,
    }

    public States currentBaseState;
    public States currentSupportState;
    public States currentAttackState;

    private Dictionary<string, GameObject> particleEffects;

    public void Start()
    {
        dragonAttack = GetComponent<DragonAttack>();
        dragonSupport = GetComponent<DragonSupport>();
        dragonGuide = GetComponent<DragonGuide>();
        dragonBehaviourController = GetComponent<DragonBehaviourController>();

        currentBaseState = States.Support;
        currentSupportState = States.Follow;
        currentAttackState = States.Damage;

        particleEffects = new Dictionary<string, GameObject>
        {
            { "Damage", dragonAttack.dmgParticles },
            { "Slow", dragonAttack.slowParticles },
            { "Heal", dragonSupport.healParticles },
            { "BlueFire", dragonGuide.blueFireBall }
        };
    }

    public void SetState(States state)
    {
        currentBaseState = state;
    }

    public void SetAttackState(States state)
    {
        currentAttackState = state;
    }

    public void SetSupportState(States state)
    {
        currentSupportState = state;
    }

    // the dragon alternates between two base states. The sub states define what the dragon actually does and work only when it is in the appropriate base state
    public void Update()
    {
        switch (currentBaseState)
        {
            case States.Attack:
                dragonAttack.AttackEnemy();
                switch (currentAttackState)
                {
                    case States.Damage:
                        SwitchParticleEffect("Damage");
                        dragonAttack.DamageEnemy();
                        dragonBehaviourController.wandering = false;
                        break;

                    case States.Slow:
                        SwitchParticleEffect("Slow");
                        dragonAttack.SlowEnemy();
                        break;

                    case States.Distract:
                        SwitchParticleEffect("Off");
                        dragonBehaviourController.wandering = true;
                        break;

                    default:
                        break;
                }
                break;

            case States.Support:
                switch (currentSupportState)
                {
                    case States.Heal:
                        dragonSupport.FollowPlayer();
                        SwitchParticleEffect("Heal");
                        break;

                    case States.Follow:
                        dragonSupport.FollowPlayer();
                        SwitchParticleEffect("Off");
                        break;

                    case States.Guide:
                        SwitchParticleEffect("BlueFire");
                        break;

                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    // Change the particle effects
    private void SwitchParticleEffect(string particleSystem)
    {
        foreach (var effect in particleEffects)
        {
            if (effect.Key == particleSystem)
                effect.Value.SetActive(true);
            else
                effect.Value.SetActive(false);
        }
    }
}
