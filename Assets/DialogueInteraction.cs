using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueInteraction : MonoBehaviour
{
    [SerializeField] string[] dialogueSentences;
    [SerializeField] TMPro.TextMeshProUGUI textObj;
    [SerializeField] Material inactiveMaterial;
    [SerializeField] Material activeMaterial;
    [SerializeField] GameObject gatterObject;
    Haptics currHaptics;
    int dialogueIndex = -1;
    bool interactionActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("PlayerSword") && interactionActive)
        {
            dialogueIndex += 1;
            if (dialogueIndex == dialogueSentences.Length-1)
            {
                gatterObject.transform.position = new Vector3(gatterObject.transform.position.x, 3.65f, gatterObject.transform.position.z);
                Destroy(this.gameObject);
                Destroy(this);
            }
            currHaptics = other.transform.parent.GetComponent<Haptics>();
            currHaptics.Vibrate(0.125f, 220, 0.75f);
            interactionActive = false;
            this.GetComponent<MeshRenderer>().material = inactiveMaterial;
            NextSentence();
            StartCoroutine(WaitForSeconds(1.0f));
        }
    }


    void NextSentence()
    {
        textObj.SetText(dialogueSentences[dialogueIndex]);
    }
    IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        interactionActive = true;
        this.GetComponent<MeshRenderer>().material = activeMaterial;
    }
}
