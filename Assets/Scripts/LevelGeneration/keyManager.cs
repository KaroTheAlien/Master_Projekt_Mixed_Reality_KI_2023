//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class keyManager : MonoBehaviour
//{
//    public Material material;
//    bool bKeyActivated = false;
//    bool playerSwordCollides = false;
//    Haptics currHaptics = null;
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (playerSwordCollides)
//        {
//            playerSwordCollides = false;
//            this.gameObject.transform.Rotate(new Vector3(0, 0, 180));


//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if(!bKeyActivated)
//        {

//        if (other.gameObject.tag.Equals("PlayerSword"))
//        {
//            playerSwordCollides = true;
//            bKeyActivated = true;
//            //todo sound abspielen?
//            currHaptics = other.transform.parent.GetComponent<Haptics>();
//            currHaptics.Vibrate(0.125f, 220, 0.75f);
//        }
//        }

//    }

//    public bool getKeyStatus()
//    {
//        return bKeyActivated;
//    }
//}

