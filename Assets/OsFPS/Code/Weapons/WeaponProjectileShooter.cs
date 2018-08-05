using UnityEngine;
using System.Collections;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Weapon behaviour that binds to <see cref="Weapon.weaponFire"/> in order to handle firing a bullet.
    /// </summary>
    [RequireComponent(typeof(Weapon))]
    public class WeaponProjectileShooter : MonoBehaviour
    {
        public Weapon weapon
        {
            get
            {
                if (this._weapon == null)
                    this._weapon = GetComponent<Weapon>();
                return this._weapon;
            }
        }
        private Weapon _weapon;

        public float baseSpread { get { return this.weapon.weaponDefinition.spread; } }
        [Header("Spread")]
        public float movingSpreadModifier = 4;
        public float crouchedSpreadModifier = 0.4f;
        /// <summary>
        /// Spread will not instantly change state after leaving movement state, instead it will linearly interpolate using lerp.
        /// This intensity is multiplied with Time.deltaTime and used as lerp parameter t.
        /// </summary>
        public float spreadLerpIntensity = 10;

        [Header("Projectile")]
        public Transform projectileOrigin;

        /// <summary>
        /// The prefab for projectile fired by this shooter.
        /// </summary>
        public GameObject projectile;

        [Header("Debug")]
        [SerializeField]
        private float spread;

        public void Update()
        {
            float spreadTarget = this.baseSpread;

            // States
            if (this.weapon.weaponHandler.entity.model.crouch.IsActive())
                spreadTarget *= this.crouchedSpreadModifier;

            if (this.weapon.weaponHandler.entity.model.motorMovement.Get().magnitude > 0.01f)
                spreadTarget *= this.movingSpreadModifier;

            // Lerp spread
            this.spread = Mathf.Lerp(this.spread, spreadTarget, Time.deltaTime * this.spreadLerpIntensity);
        }

        public void Awake()
        {
            this.weapon.weaponFire.onStart += this.OnFire;
            this.spread = this.baseSpread;
        }

        private void OnFire()
        {
            // Debug spread
            Debug.DrawLine(this.projectileOrigin.position, this.projectileOrigin.position + (this.projectileOrigin.forward * 15f), Color.red, 50f);

            // Consume ammo
            this.weapon.ammoInClip--;

            // Calculate spread
            Quaternion rot = this.projectileOrigin.rotation;
            float spreadRngBy2 = ((Random.value * 2f) - 1f) / 2f;
            Vector3 euler = rot.eulerAngles;
            euler.x += this.spread * spreadRngBy2;
            euler.y += this.spread * spreadRngBy2;
            rot = Quaternion.Euler(euler);

            // Debug spread
            Debug.DrawLine(this.projectileOrigin.position, this.projectileOrigin.position + (rot * Vector3.forward * 15f), Color.green, 50f);

            // Spawn projectile
            var p = PrefabPool.instance.GetInstance(this.projectile);
            p.transform.position = this.projectileOrigin.position;
            p.transform.rotation = rot;
        }
    }
}