using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Hitscan projectile implementation that flies in a straight line.
    /// Its position is updated every frame and a raycast check is done for every frame in order to determine whether or not something was hit.
    /// The projectile flies until it traveled <see cref="range"/> units.
    /// </summary>
    public class HitScanProjectile : MonoBehaviour
    {
        [Header("Flight & Hit config")]
        public LayerMask hitMask;
        public float flightSpeed;
        public float range;

        [Header("Damage")]
        public float damage = 10;
        public float physicalForce;

        [Header("Debug")]
        [SerializeField]
        private float _traveled;

        public void Awake()
        {
            this.Update();
        }

        public void Update()
        {
            if (this._traveled > this.range)
            {
                Destroy(this.gameObject);
                return;
            }

            float moved = this.flightSpeed * Time.deltaTime;
            RaycastHit rh;
            if (Physics.Raycast(this.transform.position, this.transform.forward, out rh, moved, this.hitMask))
            {
                rh.collider.gameObject.SendDamage(DamageEventArgs.Create(this.damage, rh.point, rh.normal, this.physicalForce));
                PrefabPool.instance.Return(this.gameObject);
            }

            this._traveled += moved;
            this.transform.position = this.transform.position + (this.transform.forward * moved);
        }
    }
}