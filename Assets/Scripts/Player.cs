using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int hp = 100;
    [SerializeField] GameObject playerSwordRight;
    [SerializeField] GameObject playerSwordLeft;
    [SerializeField] GameObject damageVisualPlane;
    Material bloodMaterial;
    Color bloodColor;
    // player gets hit and takes damage (triggerd by deactivation of the attack collider if the player didnt parry it)
    public void TakeDamage(int dmg)
    {
//        hp -= dmg; // TODO remove comment -> player is invulnerable for now
        VisualEffectDamage();
        Debug.Log(hp);
        if (hp <= 0)
        {
            Die();
        }
    }

    private void VisualEffectDamage()
    {
        bloodMaterial = damageVisualPlane.GetComponent<MeshRenderer>().material;
        bloodColor = bloodMaterial.color;
        bloodMaterial.color = new Vector4(bloodColor.r, bloodColor.g,bloodColor.b, 0.5f);
        StartCoroutine(LerpBloodColor());
    }
    IEnumerator LerpBloodColor()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        bloodMaterial.color = new Vector4(bloodColor.r, bloodColor.g, bloodColor.b, 0);
        damageVisualPlane.GetComponent<MeshRenderer>().material = bloodMaterial;
    }
    // player dies ( right now deactivate the swords ... todo game over screen)
    private void Die()
    {
        playerSwordRight.SetActive(false);
    }

    public int GetHP()
    {
        return hp;
    }

    public void SetHP(int health)
    {
        hp = health;
    }
}
