using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Base implementation of a weapon handler for entities.
    /// The weapon handler controls:
    /// - Weapon pickup
    /// - Weapon interaction
    /// -> Fire
    /// -> Reload
    /// -> Set fire mode
    /// </summary>
    public abstract class EntityWeaponHandler : EntityComponent
    {
        [Header("Debug")]
        public List<Weapon> weapons = new List<Weapon>();
        public Weapon currentWeapon;

        /// <summary>
        /// Handles the weapon firing.
        /// This is LateUpdate(), because the weapon is being unlocked in <see cref="Weapon.Update"/>.
        /// In order to not "waste" a frame of time, we try firing in the late update.
        /// </summary>
        public virtual void LateUpdate()
        {
            if (this.isFiring && this.currentWeapon != null)
            {
                if (this.currentWeapon.CanFire())
                    this.currentWeapon.Fire();

                if (this.currentWeapon.ammoInClip <= 0)
                    this.isFiring = false;
            }
        }

        public override void OnRegisterEventHandlers()
        {
            // Values and simple events
            this.entity.model.selectWeapon.RegisterCondition(CanSelectWeapon);
            this.entity.model.selectWeapon.onFire += OnSelectWeapon;
            this.entity.model.selectWeaponByIndex.RegisterCondition(CanSelectWeaponByIndex);
            this.entity.model.selectWeaponByIndex.onFire += OnSelectWeaponByIndex;
            this.entity.model.currentWeaponDefinition.SetGetter(this.GetCurrentWeaponDefinition);
            this.entity.model.currentWeapon.SetGetter(this.GetCurrentWeapon);
            this.entity.model.fireMode.SetGetter(this.GetFireMode);
            this.entity.model.availableFireModes.SetGetter(this.GetFireModes);
            this.entity.model.setFireMode.RegisterCondition(this.CanSetFireMode);
            this.entity.model.setFireMode.onFire += this.SetFireMode;
            this.entity.model.projectileOrigin.SetGetter(this.GetProjectileOrigin);
            this.entity.model.projectileOriginDir.SetGetter(this.GetProjectileOriginDir);
            this.entity.model.holsterWeapon.handler += this.HolsterWeapon;
            this.entity.model.onPickedupWeapon.handler += this.OnPickupWeapon;
            this.entity.model.onDroppedWeapon.handler += this.OnDropWeapon;

            // Fire
            this.entity.model.fire.RegisterStartCondition(this.CanFire);
            this.entity.model.fire.onStart += OnFire;
            this.entity.model.fire.onStop += OnFireStop;
            this.entity.model.fire.RegisterActivityGetter(IsFiring);

            // Reload
            this.entity.model.reload.RegisterStartCondition(this.CanReload);
            this.entity.model.reload.onStart += OnReload;
            this.entity.model.reload.RegisterActivityGetter(IsReloading);
        }

        #region Getters

        /// <summary>
        /// Returns the projectile origin direction.
        /// Will return transform.position if no weapon is currently equipped.
        /// </summary>
        protected virtual Vector3 GetProjectileOrigin()
        {
            if (this.currentWeapon == null || this.currentWeapon.projectileShooter == null)
                return this.transform.position;
            else
            {
                return this.currentWeapon.projectileShooter.projectileOrigin.position;
            }
        }

        /// <summary>
        /// Returns the projectile origin direction.
        /// Will return transform.forward if no weapon is currently equipped.
        /// </summary>
        protected virtual Vector3 GetProjectileOriginDir()
        {
            if (this.currentWeapon == null || this.currentWeapon.projectileShooter == null)
                return this.transform.position;
            else
            {
                return this.currentWeapon.projectileShooter.projectileOrigin.forward;
            }
        }

        /// <summary>
        /// Returns the weapon instance held for the specified definition.
        /// Will return null if there is no weapon held for the specified definition.
        /// </summary>
        protected virtual Weapon GetWeaponInstance(WeaponDefinition def)
        {
            return weapons.Where((w) => w.weaponDefinition == def).FirstOrDefault();
        }

        /// <summary>
        /// Gets the currently active weapon definition.
        /// </summary>
        protected virtual WeaponDefinition GetCurrentWeaponDefinition()
        {
            return this.currentWeapon == null ? null : this.currentWeapon.weaponDefinition;
        }

        /// <summary>
        /// Gets the currently active weapon.
        /// </summary>
        protected virtual Weapon GetCurrentWeapon()
        {
            return this.currentWeapon;
        }

        /// <summary>
        /// Gets an enumeration of all available weapon's weapon definition.
        /// </summary>
        protected virtual IEnumerable<WeaponDefinition> GetAvailableWeapons()
        {
            return this.weapons.Select((w) => w.weaponDefinition);
        }

        #endregion

        #region Weapon Pickup / Selection

        /// <summary>
        /// Called when the specified weapon needs to be created.
        /// Usually fired when the weapon was picked up.
        /// </summary>
        protected abstract bool _CreateWeapon(WeaponDefinition definition, out Weapon weaponInstance);

        /// <summary>
        /// Creates the weapon of specified definition.
        /// </summary>
        protected bool CreateWeapon(WeaponDefinition definition, out Weapon weaponInstance)
        {
            bool b = _CreateWeapon(definition, out weaponInstance);
            if (b)
                weaponInstance.Initialize(this);

            return b;
        }

        /// <summary>
        /// Handler for <see cref="EntityModel.dropWeapon"/>.
        /// Will delete the weapon instance.
        /// </summary>
        /// <param name="weapon">The weapon to drop.</param>
        public virtual void OnDropWeapon(WeaponDefinition weapon)
        {
            var w = this.GetWeaponInstance(weapon);
            if (Essentials.UnityIsNull(w))
                return;

            this.weapons.Remove(w);
            Destroy(w.gameObject);
        }

        /// <summary>
        /// Handler for picking up a weapon with ammo.
        /// <see cref="EntityModel.onPickedupWeapon"/>
        /// </summary>
        public virtual void OnPickupWeapon(WeaponDefinition weaponDefinition)
        {
            Weapon weaponInstance = GetWeaponInstance(weaponDefinition);

            // Pickup weapon and append weapon to current weapons list if successfull
            if (CreateWeapon(weaponDefinition, out weaponInstance))
                this.weapons.Add(weaponInstance);
        }

        /// <summary>
        /// Holsters / disables all weapon.
        /// Sets the current weapon reference to be null.
        /// <see cref="EntityModel.holsterWeapon"/>
        /// </summary>
        protected virtual void HolsterWeapon()
        {
            for (int i = 0; i < weapons.Count; i++)
                weapons[i].gameObject.SetActive(false);

            this.currentWeapon = null;
        }

        /// <summary>
        /// Whether or not a wepon at the specified index can be selected.
        /// <see cref="EntityModel.selectWeaponByIndex"/>
        /// </summary>
        protected virtual bool CanSelectWeaponByIndex(int index)
        {
            // Bounds and validity check
            if (index < 0 || index >= this.weapons.Count || this.weapons[index] == null)
                return false;

            return true;
        }
        /// <summary>
        /// Event handler for selecting a weapon by the weapon index.
        /// </summary>
        protected virtual void OnSelectWeaponByIndex(int index)
        {
            // Already equipped?
            if (object.ReferenceEquals(this.currentWeapon, this.weapons[index]))
                return;

            HolsterWeapon();

            this.currentWeapon = this.weapons[index];
            this.currentWeapon.gameObject.SetActive(true);
        }

        protected virtual bool CanSelectWeapon(WeaponDefinition def)
        {
            var weapon = GetWeaponInstance(def);
            return weapon != null;
        }
        /// <summary>
        /// Event handler for selecting a weapon by a weapon definition.
        /// </summary>
        protected virtual void OnSelectWeapon(WeaponDefinition def)
        {
            // Equip
            var weapon = GetWeaponInstance(def);

            if (weapon == null)
                Debug.LogError("Tried to select weapon that isnt existing on weapon handler " + def);
            else
                OnSelectWeaponByIndex(weapons.IndexOf(weapon));
        }

        #endregion

        #region Fire Mode

        /// <summary>
        /// Returns all available fire modes.
        /// </summary>
        protected virtual FireMode[] GetFireModes()
        {
            if (this.currentWeapon == null)
                return new FireMode[0];
            return this.currentWeapon.fireModes;
        }

        /// <summary>
        /// Gets the current fire mode.
        /// </summary>
        protected virtual FireMode GetFireMode()
        {
            return object.ReferenceEquals(this.currentWeapon, null) ? FireMode.NULL : this.currentWeapon.currentFireMode;
        }

        protected virtual bool CanSetFireMode(FireMode fireMode)
        {
            if (object.ReferenceEquals(this.currentWeapon, null))
                return false;

            return this.currentWeapon.CanSetFireMode(fireMode);
        }

        /// <summary>
        /// Tries setting the specified fire mode.
        /// Will return false if the fire mode isnt supported.
        /// </summary>
        protected virtual void SetFireMode(FireMode fireMode)
        {
            this.currentWeapon.SetFireMode(fireMode);
        }

        #endregion

        #region Firing

        private bool isFiring;

        protected virtual bool IsFiring()
        {
            return this.isFiring || this.currentWeapon != null && this.currentWeapon.isFiring;
        }

        protected virtual bool CanFire()
        {
            return this.currentWeapon != null && this.currentWeapon.CanFire();
        }

        protected virtual void OnFire()
        {
            if (this.currentWeapon == null)
                return;

            // Semi-auto firing
            if (this.currentWeapon.currentFireMode == FireMode.SEMI_AUTO)
                this.currentWeapon.Fire();
            // Full-auto firing (enable flag for Update())
            else if (this.currentWeapon.currentFireMode == FireMode.FULL_AUTO)
                this.isFiring = true;
        }

        protected virtual void OnFireStop()
        {
            this.isFiring = false;
        }

        #endregion

        #region Reload

        protected virtual bool IsReloading()
        {
            return this.currentWeapon != null && this.currentWeapon.isReloading;
        }

        protected virtual bool CanReload()
        {
            return this.currentWeapon != null && this.currentWeapon.CanReload();
        }

        protected virtual void OnReload()
        {
            this.currentWeapon.Reload();
        }

        #endregion
    }
}