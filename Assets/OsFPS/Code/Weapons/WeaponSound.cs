using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK.Audio;

namespace OsFPS
{
    /// <summary>
    /// Weapon sound implementation that handles fire, reload and dry fire sounds.
    /// </summary>
    [RequireComponent(typeof(Weapon))]
    public class WeaponSound : MonoBehaviour
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

        [Header("Audio")]
        public AudioEvent fireAudio;
        public AudioEvent dryFireAudio;
        public AudioEvent reloadAudio;
        public UTKAudioSource source;

        public void Awake()
        {
            this.weapon.weaponFire.onStart += this.OnWeaponFire;
            this.weapon.weaponFire.onFailStart += this.OnWeaponFireFailStart;
            this.weapon.weaponFire.onStop += this.OnWeaponFireDone;
            this.weapon.weaponReload.onStart += this.OnWeaponReload;
        }

        protected virtual void OnWeaponFireFailStart()
        {
            if (this.weapon.ammoInClip == 0)
                this.dryFireAudio.Play(this.source);
        }

        protected virtual void OnWeaponFire()
        {
            this.fireAudio.Play(this.source);
        }

        protected virtual void OnWeaponFireDone()
        {

        }

        protected virtual void OnWeaponReload()
        {
            this.reloadAudio.Play(this.source);
        }
    }
}