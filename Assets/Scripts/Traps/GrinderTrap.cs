using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrinderTrap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Transform>().tag.Equals("PlayerCollider")){
            GameObject.FindObjectOfType<Player>().TakeDamage(20);
        }
    }
}
