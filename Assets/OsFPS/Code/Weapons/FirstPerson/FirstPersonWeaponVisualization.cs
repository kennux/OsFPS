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
        [Header("Animation")]
        public new Animation animation;
        public string reloadAnimation;
        public string fireAnimation;

        public float reloadAnimationSpeed = 1;
        public float fireAnimationSpeed = 1;

        protected override void OnWeaponReload()
        {
            base.OnWeaponReload();

            if (Essentials.UnityIsNull(this.animation))
                return;

            this.animation[this.reloadAnimation].speed = this.reloadAnimationSpeed;
            this.animation.Play(this.reloadAnimation);
        }

        protected override void OnWeaponFireStart()
        {
            base.OnWeaponFireStart();

            if (Essentials.UnityIsNull(this.animation))
                return;

            this.animation[this.fireAnimation].speed = this.fireAnimationSpeed;
            this.animation.Play(this.fireAnimation);
        }
    }
}