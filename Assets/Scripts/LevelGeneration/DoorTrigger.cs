using System.Collections;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource activate;
    [SerializeField] private AudioSource deactivate;
    Haptics currHaptics = null;
    public GameObject[] doors = null;
    bool done = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GrabInteractable" || other.tag == "Player")
        {
            if (!done)
        {
            this.activate.Play();

            for (int i = 0; i < doors.Length; i++)
            {
                //StartCoroutine(MoveDoorsSmoothly(new Vector3(0, 2, 0)));
                //doors[i].transform.position = new Vector3(0, 3, 0);
                    doors[i].SetActive(false);

                }
                //currHaptics.Vibrate(0.125f, 220, 0.75f);

                done = true;

        }

        }

        //StartCoroutine(MovePressurePlate(new Vector3(0, -0.06f, 0)));
        //this.gameObject.transform.position += new Vector3(0, -0.06f, 0);

    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "GrabInteractable" || other.tag == "Player")
        {

        if (done)
        {
            this.deactivate.Play();

            for (int i = 0; i < doors.Length; i++)
            {
                    //StartCoroutine(MoveDoorsSmoothly(new Vector3(0, -2, 0)));
                    //doors[i].transform.localPosition = new Vector3(0, 0, 0);
                    doors[i].SetActive(true);

            }
            //currHaptics.Vibrate(0.125f, 220, 0.75f);
            //StartCoroutine(MovePressurePlate(new Vector3(0, 0.06f, 0)));
            //this.gameObject.transform.position -= new Vector3(0, -0.06f, 0);

            done = false;
        }
        }

    }


    private IEnumerator MoveDoorsSmoothly(Vector3 targetPosition)
    {
        float duration = 1.5f; // Adjust this value to control the speed of the door movement

        // Store initial positions of all doors
        Vector3[] initialPositions = new Vector3[doors.Length];
        for (int i = 0; i < doors.Length; i++)
        {
            initialPositions[i] = doors[i].gameObject.transform.position;
        }

        for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
        {
            float t = Mathf.Clamp01(elapsed / duration); // Calculate the interpolation factor

            // Calculate new positions for all doors
            Vector3[] newPositions = new Vector3[doors.Length];
            for (int i = 0; i < doors.Length; i++)
            {
                newPositions[i] = Vector3.Lerp(initialPositions[i], initialPositions[i] + targetPosition, t);
            }

            // Apply the new positions to all doors
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].gameObject.transform.position = newPositions[i];
            }

            yield return null;
        }
    }

    private IEnumerator MovePressurePlate(Vector3 targetPosition)
    {
        float duration = 0.5f; // Adjust this value to control the speed of the door movement

        // Store initial positions of all doors
        Vector3 initialPositions = this.gameObject.transform.position;

        for (float elapsed = 0; elapsed < duration; elapsed += Time.deltaTime)
        {
            float t = Mathf.Clamp01(elapsed / duration); // Calculate the interpolation factor

            // Calculate new positions for all doors
            Vector3 newPositions = Vector3.Lerp(initialPositions, initialPositions + targetPosition, t);

            this.gameObject.transform.position = newPositions;

            yield return null;
        }
    }
}