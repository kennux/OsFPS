using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Visualization component that helps with visualizing weapon interaction.
    /// This includes:
    /// - Muzzle flash
    /// - Animation / IK (handled by implementations of this class)
    /// - Weapon modules
    /// </summary>
    [RequireComponent(typeof(Weapon))]
    public abstract class WeaponVisualization : MonoBehaviour
    {
        public Weapon weapon
        {
            get
            {
                if (_weapon == null)
                    _weapon = GetComponent<Weapon>();
                return _weapon;
            }
        }
        private Weapon _weapon;

        /// <summary>
        /// Muzzle flash game object.
        /// Will be enabled to visualize the muzzleflash.
        /// </summary>
        [Header("Configs")]
        public GameObject muzzleFlash;
        public float flashTime = 0.02f;

        /// <summary>
        /// Transform from where shells are being ejected out of the weapon.
        /// </summary>
        public Transform shellEjectTransform;

        /// <summary>
        /// The shell prefab.
        /// </summary>
        public GameObject shellPrefab;

        public void Awake()
        {
            this.weapon.weaponFire.onStart += this.OnWeaponFireStart;
            this.weapon.weaponFire.onStop += this.OnWeaponFireDone;
            this.weapon.weaponReload.onStart += this.OnWeaponReload;
        }

        protected virtual void OnWeaponFireStart()
        {
            this.muzzleFlash.transform.localEulerAngles = new Vector3(this.muzzleFlash.transform.localEulerAngles.x, this.muzzleFlash.transform.localEulerAngles.y, Random.Range(0, 360));
            this.muzzleFlash.SetActive(true);

            // TODO: Get rid of the coroutine (due to memory allocations)
            this.StartCoroutine(Essentials.DelayedInvokeRoutine(() =>
            {
                this.muzzleFlash.SetActive(false);
            }, this.flashTime));

            // Shell eject
            var s = PrefabPool.instance.GetInstance(this.shellPrefab);
            s.transform.position = this.shellEjectTransform.position;
            s.transform.rotation = this.shellEjectTransform.rotation;
            s.SendMessage("InitShell");
        }

        /// <summary>
        /// Called in order to visualize a weapon reload.
        /// Will most likely play an animation.
        /// </summary>
        protected virtual void OnWeaponReload()
        {

        }

        protected virtual void OnWeaponFireDone()
        {
        }
    }
}