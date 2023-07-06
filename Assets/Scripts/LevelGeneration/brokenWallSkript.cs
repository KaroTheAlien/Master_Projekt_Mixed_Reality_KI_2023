using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class brokenWallSkript : MonoBehaviour
{
    GameObject[] childPieces;
    int length;
    public float forceAmount;
    //string ballTag = "Ball"; // Der Tag der Kugel, auf die die Kraft übertragen werden soll
    string ballTag = "GrabInteractable"; // Der Tag der Kugel, auf die die Kraft übertragen werden soll
    string playerTag = "PlayerCollider"; // Der Tag der Kugel, auf die die Kraft übertragen werden soll
    string playerTag2 = "Player"; // Der Tag der Kugel, auf die die Kraft übertragen werden soll
    Collider mainCollider;
    Collider childCollider;

    int childCount = 0;
    List<int> childIndices = new List<int>();
    int[] inactiveChildIndices;

    private void Start()
    {

        childCount = gameObject.transform.childCount;
        inactiveChildIndices = new int[childCount/3];
        activateRndObjects(this.gameObject);

    }
    // Mischt eine Liste
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    private void activateRndObjects(GameObject gameObject)
    {
        for (int i = 0; i < childCount /3; i++)
        {
            inactiveChildIndices[i] = Random.Range(0, childCount);
        }

        // Erstelle eine Liste der Indizes aller Kindobjekte
        for (int j = 0; j < childCount; j++)
        {
            childIndices.Add(j);
        }
        // Mische die Liste der Kindindizes
        ShuffleList(childIndices);

        // Aktiviere eine zufaellige Anzahl von Kindobjekten
        for (int i = 0; i < childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            if (inactiveChildIndices.Contains(i))
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
            else
                child.gameObject.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(ballTag))
        {
            Rigidbody[] childRigidbodies = GetComponentsInChildren<Rigidbody>();
            // Den Haupt-Collider der Wand finden
            mainCollider = GetComponent<Collider>();
            //forceAmount = collision.relativeVelocity.magnitude;
            // Den Haupt-Collider deaktivieren
            if (mainCollider != null)
            {
                mainCollider.enabled = false;
            }

            foreach (Rigidbody childRigidbody in childRigidbodies)
            {
                childRigidbody.isKinematic = false; // Rigidbodies aktivieren, um auf die Kraft zu reagieren
                childRigidbody.useGravity = true;
                //Vector3 forceDirection = collision.GetContact(0).point -  childRigidbody.transform.position; // Richtung der Kraft
                Vector3 forceDirection = childRigidbody.transform.position - collision.GetContact(0).point; // Richtung der Kraft

                // Anwendung der Kraft auf das Kind-Objekt der Wand
                childRigidbody.velocity = forceDirection.normalized * forceAmount;
                //childRigidbody.AddForce(forceDirection.normalized * forceAmount, ForceMode.Impulse);


                StartCoroutine(BlinkDespawn(childRigidbody.gameObject));
            }




        }


    }

    IEnumerator BlinkDespawn(GameObject gameObject)
    {
        yield return new WaitForSeconds(1f); // Wartezeit von einer Sekunde
        float blinkInterval = 0.2f; // Intervall der Blinkanimation
        float blinkDuration = 0.5f; // Gesamtdauer der Blinkanimation
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            // Deaktiviere das Spielobjekt
            gameObject.SetActive(false);
            // Warte für die Hälfte des Blinkintervalls
            yield return new WaitForSeconds(blinkInterval * 0.5f);
            // Aktiviere das Spielobjekt
            gameObject.SetActive(true);
            // Warte für die andere Hälfte des Blinkintervalls
            yield return new WaitForSeconds(blinkInterval * 0.5f);

            elapsedTime += blinkInterval;
        }

        // Setze das Spielobjekt dauerhaft auf inaktiv
        gameObject.SetActive(false);
        Destroy(gameObject);
    }   

}
