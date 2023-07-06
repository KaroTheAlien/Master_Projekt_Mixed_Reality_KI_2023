using UnityEngine;

namespace AI_Scripts
{
    public class EnemyGroupBehaviourController : MonoBehaviour
    {
        private Enemy currentAttacker { get; set; }
        private Enemy lastAttacker { get; set; }
        private int _timeout;
        private const int TimeoutLimit = 60;
        private bool otherAttackersWaiting = false;

        public bool CanAttack(Enemy enemy)
        {
            //Debug.LogWarning($"canAttack checked for: {enemy.GetInstanceID()}");
            if (this.currentAttacker == enemy)
            {
                Debug.LogWarning(
                    $"Attack requested by current attacker {enemy}. " +
                    "Post-attack cleanup might be incorrect."
                );
                return true;
            }
            if (this.currentAttacker is not null)                                           // attack option is unavailable
            {
                this.otherAttackersWaiting = true;
                return false;
            }
            if (this.lastAttacker != enemy) return true;                                    // did not attack recently
            if (this.otherAttackersWaiting && this._timeout++ < TimeoutLimit) return false; // did attack recently and did not await timeout yet
            this._timeout = 0;
            this.otherAttackersWaiting = false;
            this.lastAttacker = null;
            return true;
        }

        public bool RequestAttack(Enemy enemy)
        {
            lock (this)
            {
                if (!this.CanAttack(enemy)) return false;
                //Debug.LogWarning($"updated attacker: {enemy.name} {enemy.GetInstanceID()} Timeout: {this._timeout} multi: {this.otherAttackersWaiting}");
                this.currentAttacker = enemy;
                this.lastAttacker = this.currentAttacker;
                this.otherAttackersWaiting = false;
                this._timeout = 0;
            }
            
            return true;
        }

        public void ClearAttack()
        {
            //Debug.LogWarning("ATTACK CLEARED");
            if (this.currentAttacker is null) return; // already cleared
            this.currentAttacker = null;
            this.otherAttackersWaiting = false;
        }
    }
}
