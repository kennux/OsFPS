using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Weapon visualization implementation for first person weapons.
    /// Plays reload / on fire animation to visualize those events.
    /// </summary>
    public class FirstPersonWeaponVisualization : WeaponVisualization
    {
        /// <summary>
        /// The nimation component to use.
        /// </summary>
        [Header("Animation")]
        public new Animation animation;

        /// <summary>
        /// The reload animation name.
        /// </summary>
        public string reloadAnimation;

        /// <summary>
        /// Firing animation name.
        /// </summary>
        public string fireAnimation;

        /// <summary>
        /// Reload animation speed multiplicator.
        /// </summary>
        public float reloadAnimationSpeed = 1;

        /// <summary>
        /// Reload animation speed multiplicator.
        /// </summary>
        public float fireAnimationSpeed = 1;

        /// <summary>
        /// Animation state for when the weapon is empty.
        /// </summary>
        [Header("Animation (Optional)")]
        public string emptyState;

        /// <summary>
        /// Reload animation for when the weapon was shot empty.
        /// </summary>
        public string emptyReloadAnimation;

        protected override void OnWeaponReload()
        {
            base.OnWeaponReload();

            if (Essentials.UnityIsNull(this.animation))
                return;

            // Decide which animation to use
            var animation = this.reloadAnimation;
            if (this.weapon.ammoInClip <= 0 && !string.IsNullOrEmpty(this.emptyReloadAnimation))
                animation = this.emptyReloadAnimation;

            this.animation[animation].speed = this.reloadAnimationSpeed;
            this.animation.Play(animation);
        }

        protected override void OnWeaponFireStart()
        {
            base.OnWeaponFireStart();

            if (Essentials.UnityIsNull(this.animation))
                return;

            // Decide which animation to use
            var animation = this.fireAnimation;
            if (this.weapon.ammoInClip <= 1 && !string.IsNullOrEmpty(this.emptyState))
                animation = this.emptyState;

            this.animation[animation].speed = this.fireAnimationSpeed;
            this.animation.Play(animation);
        }
    }
}