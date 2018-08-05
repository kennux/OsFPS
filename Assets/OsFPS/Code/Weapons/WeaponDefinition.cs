using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// "high-level" definition file for weapons.
    /// This defines weapons and the prefabs to use for first / 3rd person.
    /// </summary>
    [CreateAssetMenu(menuName = "OsFPS/Weapon Definition")]
    public class WeaponDefinition : ScriptableObject
    {
        public FirstPersonWeapon firstPersonWeapon;
        public GameObject pickup;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (this.firstPersonWeapon != null)
                this.firstPersonWeapon.weaponDefinition = this;
        }
#endif

        [Header("Recoil")]
        /// <summary>
        /// The recoil pattern minimum value.
        /// <see cref="recoil"/>
        /// </summary>
        public Vector2 recoilPatternMin;

        /// <summary>
        /// The recoil pattern maximum value.
        /// <see cref="recoil"/>
        /// </summary>
        public Vector2 recoilPatternMax;

        /// <summary>
        /// The accuracy of the weapon.
        /// This is used for the weapon spread, spread is calculated as 1 - accuracy.
        /// Spread application is implemented in <see cref="WeaponProjectileShooter"/> by offsetting the ray origin by random amount (max. spread) on x and y euler angles.
        /// </summary>
        [Header("Attributes")]
        [Range(0, 1)]
        public float accuracy;
        public float spread
        {
            get { return 1f - this.accuracy; }
        }

        /// <summary>
        /// By how much does the weapon scale the movement speed
        /// </summary>
        [Range(0, 1)]
        public float mobility;

        /// <summary>
        /// Scales the recoil pattern values.
        /// 
        /// The recoil pattern values are look direction euler offsets.
        /// In the first person view those are applied directly to the camera local euler angles on every shot fired.
        /// </summary>
        [Range(0, 1)]
        public float recoil;

        /// <summary>
        /// The amount of bullets that can be fired in 1 second.
        /// This value divided by 10 is used as the shooting cooldown.
        /// </summary>
        public float fireRate;

        /// <summary>
        /// The reloading cooldown.
        /// </summary>
        public float reloadCooldown = 1;

        public float shootingCooldown { get { return 1f / this.fireRate; } }

        public FireMode[] fireModes;
    }
}