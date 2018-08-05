using UnityEngine;
using System.Collections;
using UnityTK.BehaviourModel;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Base class for every kind of weapon.
    /// Defines basic functionality such as firing, reloading, cooldowns, etc.
    /// </summary>
    public abstract class Weapon : MonoBehaviour
    {
        /// <summary>
        /// The weapon visualization.
        /// </summary>
        public WeaponVisualization visualization
        {
            get { return this._visualization.Get(this); }
        }
        private LazyLoadedComponentRef<WeaponVisualization> _visualization;

        /// <summary>
        /// The projectile shooter of the weapon.
        /// </summary>
        public WeaponProjectileShooter projectileShooter
        {
            get { return this._projectileShooter.Get(this); }
        }
        private LazyLoadedComponentRef<WeaponProjectileShooter> _projectileShooter;

        /// <summary>
        /// The weapon definition scriptable object.
        /// </summary>
        [Header("Config")]
        public WeaponDefinition weaponDefinition;

        /// <summary>
        /// The size of the clip in the weapon.
        /// </summary>
        public int clipSize;

        /// <summary>
        /// The shooting cooldown.
        /// </summary>
        public float shootingCooldown { get { return this.weaponDefinition.shootingCooldown; } }

        /// <summary>
        /// The reloading cooldown.
        /// </summary>
        public float reloadCooldown { get { return this.weaponDefinition.reloadCooldown; } }

        /// <summary>
        /// All fire modes available to this weapon.
        /// </summary>
        public FireMode[] fireModes { get { return this.weaponDefinition.fireModes; } }

        /// <summary>
        /// The amount of ammo for this weapon currently available.
        /// </summary>
        // TODO: Get rif of this awful chain of getter invocations :X
        public int ammo
        {
            get { return this.weaponHandler.entity.model.getWeaponAmmo.Invoke(this.weaponDefinition); }
            set { this.weaponHandler.entity.model.setWeaponAmmo.Fire(this.weaponDefinition, value); }
        }

        [Header("Debug")]
        /// <summary>
        /// The currently selected fire mode.
        /// </summary>
        public FireMode currentFireMode;

        /// <summary>
        /// The current ammo in the clip.
        /// </summary>
        public int ammoInClip;

        /// <summary>
        /// The weapon handler.
        /// </summary>
        public EntityWeaponHandler weaponHandler { get; private set; }

        /// <summary>
        /// Activity for firing the weapon.
        /// </summary>
        public ModelActivity weaponFire = new ModelActivity();

        /// <summary>
        /// Activity for reloading the weapon.
        /// </summary>
        public ModelActivity weaponReload = new ModelActivity();

        /// <summary>
        /// Whether or not the weapon is currently busy with something (atm either shooting or reloading).
        /// </summary>
        public bool isBusy { get { return this.isReloading || this.isFiring; } }

        /// <summary>
        /// Whether or not this weapon is currently reloading.
        /// </summary>
        public bool isReloading { get; private set; }

        /// <summary>
        /// Whether or not this weapon is currently firing.
        /// </summary>
        public bool isFiring { get; private set; }

        public void Awake()
        {
            this.weaponFire.RegisterActivityGetter(this.IsFiring);
            this.weaponReload.RegisterActivityGetter(this.IsReloading);

            this.weaponFire.RegisterStartCondition(this._CanFire);
            this.weaponReload.RegisterStartCondition(this._CanReload);
        }

        public bool CanSetFireMode(FireMode fireMode)
        {
            if (System.Array.IndexOf(this.fireModes, fireMode) == -1)
                return false;
            return true;
        }

        public void SetFireMode(FireMode fireMode)
        {
            if (System.Array.IndexOf(this.fireModes, fireMode) == -1)
                return;
            this.currentFireMode = fireMode;
        }

        public void Initialize(EntityWeaponHandler weaponHandler)
        {
            this.weaponHandler = weaponHandler;
        }

        #region Reloading

        private bool IsReloading()
        {
            return this.isReloading;
        }

        protected virtual bool _CanReload()
        {
            return this.ammo > 0 && this.ammoInClip < this.clipSize && !this.isBusy;
        }

        public bool CanReload()
        {
            return this.weaponReload.CanStart();
        }

        /// <summary>
        /// Tries to reload this weapon.
        /// </summary>
        /// <returns></returns>
        public virtual void Reload()
        {
            // The weapon was just reloaded
            this.OnWeaponReload();

            // Delayed on reload done event firing
            // TODO: Get rid of the coroutine (due to memory allocations)
            this.StartCoroutine(Essentials.DelayedInvokeRoutine(this.OnWeaponReloadDone, this.reloadCooldown));
        }

        /// <summary>
        /// Called when weapon reloading started from <see cref="Reload"/>.
        /// </summary>
        protected virtual void OnWeaponReload()
        {
            this.isReloading = true;
            this.weaponReload.ForceStart();
        }

        /// <summary>
        /// Called when the weapon was fully reloaded.
        /// </summary>
        protected virtual void OnWeaponReloadDone()
        {
            this.weaponReload.ForceStop();

            this.ammo += this.ammoInClip;
            this.ammoInClip = Mathf.Min(this.clipSize, this.ammo);
            this.ammo -= this.ammoInClip;
            this.isReloading = false;
        }

        #endregion

        #region Firing

        private float fireCooldownOver;
        /// <summary>
        /// Updates the weapon fire cooldown.
        /// </summary>
        public void Update()
        {
            if (this.fireCooldownOver == -1)
                return;

            if (Time.time + Time.deltaTime >= this.fireCooldownOver)
            {
                this.OnWeaponFireDone();
                this.fireCooldownOver = -1;
            }
        }

        private bool IsFiring()
        {
            return this.isFiring;
        }

        protected virtual bool _CanFire()
        {
            return this.ammoInClip > 0 && !this.isBusy;
        }

        public bool CanFire()
        {
            return !this.isBusy;
        }

        /// <summary>
        /// Tries to fire this weapon.
        /// </summary>
        /// <returns></returns>
        public virtual void Fire()
        {
            // The weapon was just fired
            this.OnWeaponFire();

            // Delayed on fired done event firing
            // See Update(), this is not accurate enough :-'(
            // this.StartCoroutine(Utils.DelayedInvokeRoutine(this.OnWeaponFireDone, this.shootingCooldown));
            this.fireCooldownOver = Time.time + this.shootingCooldown;
        }

        /// <summary>
        /// Called when the weapon was just fired from <see cref="Fire"/>
        /// </summary>
        protected virtual void OnWeaponFire()
        {
            this.weaponFire.TryStart();
            this.isFiring = true;
        }

        /// <summary>
        /// Called when the weapon firing is done, this interval between this and <see cref="OnWeaponFire"/> is defined here <see cref="shootingCooldown"/>.
        /// </summary>
        protected virtual void OnWeaponFireDone()
        {
            this.weaponFire.ForceStop();
            this.isFiring = false;
        }

        #endregion
    }
}