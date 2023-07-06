using UnityEngine;

public class AnimationEventCatcher : MonoBehaviour
{
    public void OnEnemySoundEvent(string msg)
    {
        this.GetComponentInParent<EnemyEventHandler>().OnEnemySoundEvent(msg);
    }
    
    public void OnEnemyAnimationEvent(string msg)
    {
        
        this.GetComponentInParent<EnemyEventHandler>().OnEnemyAnimationEvent(msg);
    }
}
