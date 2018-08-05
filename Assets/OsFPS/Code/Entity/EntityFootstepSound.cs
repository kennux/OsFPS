using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK.Audio;

namespace OsFPS
{
    /// <summary>
    /// Simple footstep sound implementation that plays a sound based on whether or not the entity is currently in prone state whenever <see cref="EntityModel.footstep"/> is fired.
    /// </summary>
    public class EntityFootstepSound : EntityComponent
    {
        /// <summary>
        /// The audio source used to play back the sound.
        /// </summary>
        public UTKAudioSource footstepSource;

        /// <summary>
        /// The audio played on regular footsteps.
        /// </summary>
        public AudioEvent footstepAudio;

        /// <summary>
        /// The audio played on proned footsteps.
        /// </summary>
        public AudioEvent proneFootstepAudio;

        public override void OnRegisterEventHandlers()
        {
            this.entity.model.footstep.handler += this.OnFootstep;
        }

        private void OnFootstep(Foot foot)
        {
            (this.entity.model.prone.IsActive() ? this.proneFootstepAudio : this.footstepAudio).Play(this.footstepSource);
        }
    }
}