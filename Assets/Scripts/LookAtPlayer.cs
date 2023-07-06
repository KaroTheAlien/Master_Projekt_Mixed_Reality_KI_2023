using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] GameObject neckBone;
    [SerializeField] GameObject playerObject;
    [SerializeField] bool needLimit = true;

    float currLerp;
    // Start is called before the first frame update
    void Start()
    {
        if (!playerObject)
        {
            playerObject = FindObjectOfType<Camera>().gameObject;
        }
        currLerp = 0.01f;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        Quaternion neckBoneRotation = neckBone.transform.rotation;
        Quaternion neckBoneParentRotation = neckBone.transform.parent.rotation;  
        //Debug.Log(Vector3.Angle(transform.parent.position - playerObject.transform.position, transform.parent.forward));
        if (Vector3.Angle(transform.parent.position-playerObject.transform.position, transform.parent.forward) >100 || !needLimit)
        {
            if (currLerp < 1)
            {
                currLerp += 0.025f;
            }
            Vector3 lerp = (neckBone.transform.forward + currLerp* (playerObject.transform.position - neckBone.transform.position)).normalized;   
            neckBone.transform.rotation = Quaternion.LookRotation(lerp);
        }
        else 
        {
            if (currLerp > 0)
            {
                currLerp -= 0.025f;
            }
            Vector3 lerp = ( neckBone.transform.forward +  currLerp*(playerObject.transform.position - neckBone.transform.position)).normalized;
            neckBone.transform.rotation = Quaternion.LookRotation(lerp);
        }
    }
}
