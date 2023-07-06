using System;
using UnityEngine;

namespace AI_Scripts
{
    public class EnemySfx : MonoBehaviour
    {
        public AudioSource[] weaponParriedSounds;
        public AudioSource[] enemyParriedSounds;
        public AudioSource[] weaponAttackSounds;
        public AudioSource[] enemyAttackSounds;
        public AudioSource[] aggroSounds;
        public AudioSource[] defeatSounds;
        private Enemy _enemyObject;

        private void Awake()
        {
            this._enemyObject = this.GetComponent<Enemy>();
        }

        public void PlaySound(Type type, float delay = 0.0f)
        {
            AudioClip selected = type switch
            {
                Type.WEAPON_PARRIED => this.weaponParriedSounds.Random()?.clip,
                Type.ENEMY_PARRIED  => this.enemyParriedSounds.Random()?.clip,
                Type.WEAPON_ATTACK  => this.weaponAttackSounds.Random()?.clip,
                Type.ENEMY_ATTACK   => this.enemyAttackSounds.Random()?.clip,
                Type.AGGRO          => this.aggroSounds.Random()?.clip,
                Type.DEFEAT         => this.defeatSounds.Random()?.clip,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            if (selected is null) return;
            this.PlaySound(selected, delay);
        }

        private void PlaySound(AudioClip audioClip, float delay = 0.0f)
        {
            if (audioClip is null) return;
            if (delay < 0.001f)
            {
                AudioSource.PlayClipAtPoint(audioClip, this._enemyObject.transform.position);
            } else
            {
                this.StartCoroutine(this.PlaySoundWithDelay(audioClip, delay));
            }
        }
        
        public enum Type { WEAPON_PARRIED, ENEMY_PARRIED, WEAPON_ATTACK, ENEMY_ATTACK, AGGRO, DEFEAT }
    }
}
