using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Very simple damage handler that immmediately despawns the object on death and spawns a new one.
    /// Cane be used for simple objects like explosive barrels.
    /// </summary>
    public class SimpleDamageHandler : MonoBehaviour, IDamageHandler
    {
        [Header("Config")]
        public float health;

        [Header("Death")]
        public GameObject spawnOnDeath;
        public bool destroyOnDeath = true;
        public float destroyDelay = 0;

        public void TakeDamage(DamageEventArgs args)
        {
            this.health -= args.damage;

            if (this.health <= 0)
                Die();
        }

        private void Die()
        {
            if (this.spawnOnDeath != null)
                Instantiate(this.spawnOnDeath, this.transform.position, this.transform.rotation);

            if (this.destroyOnDeath)
                Destroy(this.gameObject, this.destroyDelay);
        }

    }
}