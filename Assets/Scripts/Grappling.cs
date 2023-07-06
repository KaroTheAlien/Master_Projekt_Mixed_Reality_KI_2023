using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    public Transform contr;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
    private Transform hitObj;
    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;
    private bool grabbing;
    private GameObject origin; 
    
    private void Awake()
    {
        lr.enabled = true;
        grapplePoint = gunTip.position + contr.forward * maxGrappleDistance;
        lr.SetPosition(0, grapplePoint);
        lr.startColor = Color.white;
        lr.endColor = Color.white;
    }

    private void Start()
    {
        grabbing = false;
        origin = null;
    //lr.SetWidth(0.1f, 0.1f);
}

    private void ToggleGrappleRenderMat()
    {
        if (lr.startColor == Color.white)
        {
            lr.startColor = Color.black;
            lr.endColor = Color.black;
           //lr.SetWidth(0.1f, 0.1f);
        }
        else if (lr.startColor == Color.black)
        {
            lr.startColor = Color.white;
            lr.endColor = Color.white;
        }
    }

    private void Update()
    {
        lr.enabled = true;
        //lr.materials[0] = lr.materials[1];
        grapplePoint = gunTip.position + contr.forward * maxGrappleDistance;

        lr.SetPosition(1, grapplePoint);
        lr.SetPosition(0, gunTip.position);
        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }
    
    private void LateUpdate()
    {
       if (grappling)
          lr.SetPosition(0, gunTip.position);
    }

    public void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;
        ToggleGrappleRenderMat();
        grappling = true;
        if (GetGrabStatus())
        {
            //Transform left_hand = GameObject.Find("Grapple").transform.parent;
            //Transform projectile = left_hand.Find("Eisenkugel").transform;
            Transform projectile = null;
            foreach (Transform child in contr)
            {
                if (child.tag.Equals("GrabInteractable"))
                {
                    projectile = child.gameObject.transform;
                    break;
                }
            }
            //projectile.SetParent(origin.transform);
            //projectile.parent = null;
            //GameObject.Destroy(origin);

            Vector3 pos = gunTip.position + contr.forward * 10;
            Vector3 dir = grapplePoint - gunTip.position ;
            dir = dir.normalized;
            //StartCoroutine(MoveOverSpeed(projectile, pos, 10.0f));

            //Destroy(projectile.transform.gameObject);
            //GameObject child = projectile.GetChild(0).gameObject;
            var childs = hitObj.GetComponentsInChildren<Rigidbody>();

            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].useGravity = true;
                childs[i].isKinematic = false;
            }
            projectile.gameObject.GetComponent<Rigidbody>().useGravity = true;
            projectile.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            projectile.GetComponent<Rigidbody>().AddForce((contr.forward * 1000));
            //child.GetComponent<Rigidbody>().AddForce(pos * 2.0f);

            //child.GetComponent<Rigidbody>().useGravity = true;
            SetGrabStatus(false);

            projectile.SetParent(origin.transform);
            projectile.parent = null;
            GameObject.Destroy(origin);


            Invoke(nameof(StopGrapple), grappleDelayTime);
            return;

        }


        RaycastHit hit;
        if (Physics.Raycast(contr.position, contr.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {

            grapplePoint = hit.point;
            hitObj = hit.transform;

            //Debug.Log(hitObj.transform.gameObject.name);

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = contr.position + contr.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
        lr.enabled = true;

        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        Vector3 directionVector = (gunTip.position - grapplePoint).normalized;

        if (hitObj.transform.gameObject.tag == "GrabInteractable")
        {
           
                origin = new GameObject();

                //origin.transform = hitObj.transform.parent.transform;
                origin.transform.localPosition = hitObj.transform.localPosition;
                origin.transform.localRotation = hitObj.transform.localRotation;
                origin.transform.localScale = hitObj.transform.localScale;


            
            //else if (hitObj.transform.gameObject.name == "FussKette")
            //{
            //    origin = new GameObject();

            //    //origin.transform = hitObj.transform.parent.transform;
            //    origin.transform.localPosition = hitObj.transform.localPosition;
            //    origin.transform.localRotation = hitObj.transform.localRotation;
            //    origin.transform.localScale = hitObj.transform.localScale;


            //}
            //else if (hitObj.transform.gameObject.name == "FuﬂKette")
            //{
            //    origin = new GameObject();
            //    origin.transform.localPosition = hitObj.transform.localPosition;
            //    origin.transform.localRotation = hitObj.transform.localRotation;
            //    origin.transform.localScale = hitObj.transform.localScale;
            //}
            SetGrabStatus(true);

            hitObj.GetComponent<Rigidbody>().useGravity = false;
            hitObj.GetComponent<Rigidbody>().isKinematic = true;
            var childs = hitObj.GetComponentsInChildren<Rigidbody>();

            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].useGravity = false;
                childs[i].isKinematic = true;
            }
            //hitObj.GetComponent<Rigidbody>().isKinematic = false;


            hitObj.transform.SetParent(this.transform.parent);
            hitObj.transform.localPosition = new Vector3(0,0,0.5f);
            Vector3 pos;
           /* pos.x = this.transform.parent.position.x;
            pos.y = this.transform.parent.position.y;
            pos.z = this.transform.parent.position.z + 20.0f;
            //directionVector = (pos - hitObj.transform.position).normalized;
            hitObj.transform.position = pos;*/

            //StartCoroutine(MoveOverSpeed(hitObj.transform.parent.transform, pos, 10.0f));
        }
        //else if (hitObj.transform.gameObject.tag == "Enemy")
        //{
        //    GameObject left_hand = GameObject.Find("Grapple");

        //    Vector3 pos;
        //    pos.x = left_hand.transform.parent.position.x;
        //    pos.y = hitObj.transform.position.y;
        //    pos.z = left_hand.transform.parent.position.z - 3.0f;
        //    hitObj.transform.parent.transform.position = pos;
        //    //StartCoroutine(MoveOverSpeed(hitObj.transform.parent.transform, pos, 10.0f));

        //}
        Invoke(nameof(StopGrapple), 1f);
    }

    public void DropGrappleObject()
    {
        ToggleGrappleRenderMat();
        Transform left_hand = GameObject.Find("Grapple").transform.parent;
        //Transform projectile = left_hand.Find("FussKette").transform;
        //Transform projectile = left_hand.Find("Eisenkugel").transform;
        Transform projectile = null;
        foreach (Transform child in left_hand)
        {
            if (child.tag.Equals("GrabInteractable"))
            {
                projectile = child.gameObject.transform;
                break;
            }
        }
        SetGrabStatus(false);
        projectile.SetParent(origin.transform);
        projectile.parent = null;
        GameObject.Destroy(origin);
        projectile.gameObject.GetComponent<Rigidbody>().useGravity = true;
        projectile.gameObject.GetComponent<Rigidbody>().isKinematic = false;

    }

    public void StopGrapple()
    {
        ToggleGrappleRenderMat();
        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    private void SetGrabStatus(bool grab)
    {
        grabbing = grab;
    }

    public bool GetGrabStatus()
    {
        return grabbing;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    public IEnumerator MoveOverSpeed(Transform trans, Vector3 end, float speed = 1.0f)
    {
        float timestop = 1.0f;
        float timer = 0.0f;
        trans.gameObject.GetComponent<Rigidbody>().useGravity = false;
        GameObject child = trans.GetChild(0).gameObject;
        child.GetComponent<Rigidbody>().useGravity = false;
        while (trans.position != end && timer < timestop)
        {
            timer += Time.deltaTime;
            trans.position = Vector3.MoveTowards(trans.position, end, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator Shoot(Transform trans, Vector3 dir, float speed = 1.0f)
    {
        float timestop = 1.0f;
        float timer = 0.0f;
        // speed should be 1 unit per second
        while (timer < timestop)
        {
            timer += Time.deltaTime;
            trans.position = trans.position + (dir.normalized * speed);
            origin.transform.position = trans.position;
            yield return new WaitForEndOfFrame();
        }

    }
}