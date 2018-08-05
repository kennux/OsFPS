using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK.BehaviourModel;

namespace OsFPS
{
    /// <summary>
    /// Specialized weapon implementation for first-person weapons.
    /// </summary>
    [RequireComponent(typeof(FirstPersonWeaponAnimation))]
    public class FirstPersonWeapon : Weapon
    {
        public FirstPersonWeaponAnimation anim
        {
            get
            {
                if (_anim == null)
                    _anim = GetComponent<FirstPersonWeaponAnimation>();
                return _anim;
            }
        }
        private FirstPersonWeaponAnimation _anim;

        public new FirstPersonWeaponHandler weaponHandler
        {
            get
            {
                return base.weaponHandler as FirstPersonWeaponHandler;
            }
        }
        private FirstPersonWeaponHandler _weaponHandler;

        /// <summary>
        /// Fired in <see cref="OnFootstep"/>
        /// </summary>
        public ModelEvent<Foot> footstep = new ModelEvent<Foot>();

        /// <summary>
        /// Called from <see cref="FirstPersonWeaponHandler"/> in order to forward the event.
        /// This essentially is bound to <see cref="EntityModel.footstep"/>.
        /// </summary>
        public void OnFootstep(Foot foot)
        {
            this.footstep.Fire(foot);
        }
    }
}