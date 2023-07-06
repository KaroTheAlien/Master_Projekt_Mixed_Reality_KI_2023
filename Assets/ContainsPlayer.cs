using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContainsPlayer : MonoBehaviour
{
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("PlayerCollider")){
            SceneManager.LoadScene("MergeScene");
        }
    }
 
}
