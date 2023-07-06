using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyParent : MonoBehaviour
{
    public abstract void Parried();
    public abstract void PauseAnimation();

    public abstract void CalculatePostureDamage(float highestDistance);
}
