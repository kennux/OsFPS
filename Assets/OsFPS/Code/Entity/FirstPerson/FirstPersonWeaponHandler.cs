using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Specialized weapon handler for <see cref="FirstPersonEntity"/> entities.
    /// </summary>
    [RequireComponent(typeof(FirstPersonEntity))]
    public class FirstPersonWeaponHandler : EntityWeaponHandler
    {
        /// <summary>
        /// The transform parent where newly created weapons will be parented to.
        /// </summary>
        [Header("Config")]
        public Transform weaponParent;

        /// <summary>
        /// The flag determining whether or not the weapon is currently zoomed.
        /// </summary>
        [Header("Debug")]
        public bool isZoomed;

        /// <summary>
        /// Cached reference getter to the <see cref="FirstPersonEntity"/> of this handler.
        /// </summary>
        public FirstPersonEntity playerEntity
        {
            get
            {
                return this._playerEntity.Get(this);
            }
        }
        private LazyLoadedComponentRef<FirstPersonEntity> _playerEntity = new LazyLoadedComponentRef<FirstPersonEntity>();

        public override void OnRegisterEventHandlers()
        {
            base.OnRegisterEventHandlers();

            // Zooming
            this.playerEntity.fpModel.zoom.RegisterStartCondition(this.CanZoom);
            this.playerEntity.fpModel.zoom.RegisterActivityGetter(this.IsZoomed);
            this.playerEntity.fpModel.zoom.onStart += this.OnZoomStart;
            this.playerEntity.fpModel.zoom.onStop += this.OnZoomEnd;

            // Footsteps
            this.playerEntity.fpModel.footstep.handler += this.OnFootstep;

            this.entity.model.externalForce.handler += this.OnExternalForce;
        }

        public void Update()
        {
            if (this.currentWeapon == null)
            {
                if (this.IsZoomed())
                    this.playerEntity.fpModel.zoom.ForceStop();
            }
        }

        /// <summary>
        /// Forwards footsteps to <see cref="FirstPersonWeapon.OnFootstep"/>, bound to <see cref="EntityModel.footstep"/>.
        /// </summary>
        private void OnFootstep(Foot foot)
        {
            if (this.currentWeapon != null)
            {
                (this.currentWeapon as FirstPersonWeapon).OnFootstep(foot);
            }
        }

        protected override bool _CreateWeapon(WeaponDefinition definition, out Weapon weaponInstance)
        {
            // Instantiate
            var weaponGo = Instantiate(definition.firstPersonWeapon.gameObject, this.weaponParent);
            weaponInstance = weaponGo.GetComponent<FirstPersonWeapon>();

            // Setup transform
            weaponGo.transform.parent = this.weaponParent;
            weaponGo.transform.localPosition = Vector3.zero;
            weaponGo.transform.localRotation = Quaternion.identity;

            // Register
            weaponGo.SetActive(false);

            return true;
        }

        private void OnExternalForce(Vector3 force)
        {
            if (object.ReferenceEquals(this.currentWeapon, null))
                return;

            (this.currentWeapon as FirstPersonWeapon).anim.ApplyForce(force);
        }

        #region Zooming

        public bool CanZoom()
        {
            return this.currentWeapon != null && !this.isZoomed;
        }

        public bool IsZoomed()
        {
            return this.isZoomed;
        }

        private void OnZoomStart()
        {
            this.isZoomed = true;
        }

        private void OnZoomEnd()
        {
            this.isZoomed = false;
        }

        #endregion
    }
}