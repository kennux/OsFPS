using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// A simple entity footstep implementation that adds up traveled distance until enough was traveled to send a footstep.
    /// In order to send a footstep, <see cref="EntityModel.footstep"/> will be fired.
    /// 
    /// When in sneak mode (<see cref="EntityModel.sneak"/> is active), footsteps are being supressed.
    /// Footsteps are also being fired in prone state, tho with another interval. This can be used to keep the force animation and other things, even tho they arent really footsteps.
    /// </summary>
    public class EntityFootsteps : EntityComponent
    {
        /// <summary>
        /// The distance that must be traveled in order to fire a footstep.
        /// </summary>
        public float footstepDistance = 3f;

        /// <summary>
        /// The <see cref="footstepDistance"/> in prone state.
        /// </summary>
        public float proneFootstepDistance = 3f;

        /// <summary>
        /// Position of the entity in the last frame.
        /// </summary>
        private Vector3 lastFramePos;

        /// <summary>
        /// A temporary variable storing the distance the entity traveled since the last footstep playback.
        /// </summary>
        private float _footstepTravel;

        /// <summary>
        /// The next foot that will be used to footstep.
        /// </summary>
        private Foot nextFoot = Foot.Left;

        public void Start()
        {
            this.lastFramePos = this.transform.position;
            this._footstepTravel = 0;
        }

        public void Update()
        {
            // Sneaking completely disables footsteps
            if (this.IsSneaking())
                return;

            if (this.entity.model.grounded.Get())
                this._footstepTravel += Vector3.Scale((this.transform.position - this.lastFramePos), Vector3.forward + Vector3.right).magnitude;

            float dist = this.entity.model.prone.IsActive() ? this.proneFootstepDistance : this.footstepDistance;

            // foot stepping
            if (this._footstepTravel >= dist)
            {
                this.entity.model.footstep.Fire(this.nextFoot);
                this.nextFoot = (Foot)((int)(this.nextFoot + 1) % 2);
                this._footstepTravel = 0;
            }

            this.lastFramePos = this.transform.position;
        }

        public override void OnRegisterEventHandlers()
        {
            base.OnRegisterEventHandlers();

            // Sneaking handler
            this.entity.model.sneak.RegisterActivityGetter(this.IsSneaking);
            this.entity.model.sneak.RegisterStartCondition(this.CanSneak);
            this.entity.model.sneak.onStart += this.OnSneakStart;
            this.entity.model.sneak.onStop += this.OnSneakStop;
        }

        #region Sneaking
        // Sneaking:
        private bool isSneaking;
        private bool IsSneaking()
        {
            return this.isSneaking;
        }

        private bool CanSneak()
        {
            return !this.IsSneaking();
        }

        private void OnSneakStart()
        {
            this.isSneaking = true;
        }

        private void OnSneakStop()
        {
            this.isSneaking = false;
        }
        #endregion

    }
}