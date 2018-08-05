using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Simple helper script that will send the <see cref="EntityModel.pickupWeapon"/> event OnTriggerEnter(Collider c).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class WeaponPickup : MonoBehaviour, IInteractable
    {
        [Header("Config")]
        public InteractionParameters interactionParameters;
        public WeaponDefinition weaponDef;
        public int ammo = 300;

        public InteractionParameters parameters
        {
            get
            {
                return this.interactionParameters;
            }
        }

        Collider IInteractable.collider
        {
            get
            {
                return this._collider.Get(this);
            }
        }
        private LazyLoadedComponentRef<Collider> _collider;

        public void OnInteractionCanceled(Entity user)
        {

        }

        public void OnInteractionFinished(Entity user)
        {
            if (user != null && user.model.pickupWeapon.Try(new WeaponAmmoTuple(this.weaponDef, this.ammo))) 
            {
                Destroy(this.gameObject);
            }
        }

        public void OnInteractionStarted(Entity user)
        {

        }

    }
}