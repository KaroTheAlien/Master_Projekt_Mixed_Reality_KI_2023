//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DoorManager : MonoBehaviour
//{
//    keyManager keymanager;
//    [SerializeField] public GameObject KeyGo;
//    GameObject activeKeyInstance;
//    bool bTranslationDone = false;
//    // Start is called before the first frame update
//    void Start()
//    {
//        activeKeyInstance = GameObject.FindGameObjectWithTag("activeKey");
//        keymanager = activeKeyInstance.GetComponent<keyManager>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (!bTranslationDone)
//        {
//            if (keymanager != null)
//            {
//                if (keymanager.getKeyStatus())
//                {

//                    this.gameObject.transform.Translate(new Vector3(0, 2, 0));
//                    bTranslationDone = true;    
//                    //this.gameObject.SetActive(false);
//                }
//            }
//        }
//    }
//}
