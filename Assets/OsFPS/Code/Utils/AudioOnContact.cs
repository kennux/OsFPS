using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK.Audio;

namespace OsFPS
{
    /// <summary>
    /// Simple helper script that plays an audio event on contact with other colliders.
    /// Makes use of the OnCollisionEnter(Collision c) event.
    /// </summary>
    public class AudioOnContact : MonoBehaviour
    {
        /// <summary>
        /// The source used to play the sound.
        /// </summary>
        public UTKAudioSource audioSource;

        /// <summary>
        /// The sound that is being played on contact.
        /// </summary>
        public AudioEvent contactSound;

        public void OnCollisionEnter(Collision c)
        {
            if (this.contactSound != null)
            {
                this.contactSound.Play(this.audioSource);
            }

            this.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}