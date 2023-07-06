using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardReact : MonoBehaviour
{
    [SerializeField] GameObject speechBubble;
    Haptics currHaptics;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("PlayerSword"))
        {
            speechBubble.SetActive(true);
            other.transform.parent.GetComponent<Haptics>().Vibrate(0.125f, 220, 0.75f);
            StartCoroutine(WaitForSeconds(2.0f));
        }
    }

    IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        speechBubble.SetActive(false);
    }
}
