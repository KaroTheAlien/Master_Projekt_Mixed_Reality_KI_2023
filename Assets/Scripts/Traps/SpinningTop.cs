using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningTop : MonoBehaviour
{
    Vector3 currDirection;
    [SerializeField] float movementSpeed =0.05f;
    [SerializeField] int dmg = 10;
    [SerializeField] ParticleSystem parryParticleSystem;
    Haptics currHaptics = null;
    bool canParry = true;
    private void Start()
    {

        currDirection = new Vector3(1,0,0); 
        this.GetComponent<MoveDirection>().SetSpeed(movementSpeed);
       this.GetComponent<MoveDirection>().SetCurrDircetion(currDirection);
        parryParticleSystem = GameObject.FindGameObjectWithTag("ParticleTag").GetComponent<ParticleSystem>();


    }

    private void Update()
    {
        this.transform.Rotate(0, 0, 7.5f);
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag.Equals("PlayerCollider"))
        {
            FindObjectOfType<Player>().TakeDamage(dmg);
            currDirection *= -1;
            this.GetComponent<MoveDirection>().SetCurrDircetion(currDirection);
        }
        else if (!collision.gameObject.name.Equals("Dragon"))
        {
            int rand = Random.Range(30, 110);
            currDirection = Quaternion.AngleAxis(rand, Vector3.up) * currDirection;
            this.GetComponent<MoveDirection>().SetCurrDircetion(currDirection);
        }
        else
        {
            Physics.IgnoreCollision(collision.collider, this.GetComponent<Collider>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("PlayerSword") && canParry)
        {
            currHaptics = other.transform.parent.GetComponent<Haptics>();
            currHaptics.Vibrate(0.125f, 220, 0.75f);
            parryParticleSystem.transform.position = other.ClosestPoint(transform.position);
            parryParticleSystem.Play();
            currDirection *= -1;
            this.GetComponent<MoveDirection>().SetCurrDircetion(currDirection);
            canParry = false;
            StartCoroutine(WaitForSeconds(0.25f));
        }
    }
    IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        canParry = true;
    }
}
