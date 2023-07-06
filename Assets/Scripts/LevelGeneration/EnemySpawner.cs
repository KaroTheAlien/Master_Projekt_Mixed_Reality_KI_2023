using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    bool bSpawned = false;
    public GameObject[] otherSpawner;
    bool bEnemyActive = false;
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!bSpawned)
    //    {

    //        var enemySpawnGO = GameObject.FindGameObjectsWithTag("EnemySpawnLocation");
    //        RoomBehavior test = this.GetComponentInParent<RoomBehavior>();
    //        var activePositions = test.getactiveSpawnableChildrenPositions();
    //        //for (int i = 0; i < enemySpawnGO.Length; i++) 
    //        //{
    //        //    if (enemySpawnGO[i] == )
    //        //}
    //        for (int i = 0; i < activePositions.Length; i++)
    //        {
    //            Enemy enemy = activePositions[i].GetComponentInChildren<Enemy>(true);
    //            enemy.gameObject.SetActive(true);
    //        }
    //        bSpawned = true;
    //    }
    //}
    private void Start()
    {
        oldSpeed = new List<float> { };
    }
    List<float> oldSpeed;
    public float delayBetweenSpawns = 0.90f; // Die Verz�gerung zwischen den Aufrufen in Sekunden

    private IEnumerator SpawnEnemiesWithDelay()
    {
        var enemySpawnGO = GameObject.FindGameObjectsWithTag("EnemySpawnLocation");
        RoomBehavior test = this.GetComponentInParent<RoomBehavior>();
        var activePositions = test.getactiveSpawnableChildrenPositions();

        for (int i = 0; i < activePositions.Length; i++)
        {
            Enemy enemy = activePositions[i].GetComponentInChildren<Enemy>(true);
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);
                oldSpeed.Add(enemy.GetComponent<NavMeshAgent>().speed);
                enemy.GetComponent<NavMeshAgent>().speed = 0;

                //enemy.navAgent.speed = 0;
            }

            // yield return new WaitForSeconds(delayBetweenSpawns); // Warte f�r die angegebene Verz�gerung
        }

        yield return new WaitForSeconds(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "PlayerCollider")
        {

            if (!bSpawned)
            {
                bSpawned = true;
                for (int i = 0; i < otherSpawner.Length; i++)
                {
                    otherSpawner[i].gameObject.SetActive(false);
                }
                StartCoroutine(SpawnEnemiesWithDelay());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "PlayerCollider")
        {

            if (bSpawned)
            {
                for (int i = 0; i < otherSpawner.Length; i++)
                {
                    otherSpawner[i].gameObject.SetActive(false);
                }
                var enemySpawnGO = GameObject.FindGameObjectsWithTag("EnemySpawnLocation");
                RoomBehavior test = this.GetComponentInParent<RoomBehavior>();
                var activePositions = test.getactiveSpawnableChildrenPositions();
                for (int i = 0; i < activePositions.Length; i++)
                {
                    Enemy enemy = activePositions[i].GetComponentInChildren<Enemy>(true);
                    if (enemy != null)
                    {
                        enemy.gameObject.SetActive(true);
                        enemy.GetComponent<NavMeshAgent>().speed = oldSpeed.ToArray()[i];
                    }

                    // yield return new WaitForSeconds(delayBetweenSpawns); // Warte f�r die angegebene Verz�gerung
                }
            }
        }
    }

}
