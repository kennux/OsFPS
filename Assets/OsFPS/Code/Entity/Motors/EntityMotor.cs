using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Base class for entity motors.
    /// 
    /// Motors handle the entity movement and the collider setup.
    /// They usually bind to the activities run, jump, crouch, prone.
    /// 
    /// They read their controller input from <see cref="EntityModel.motorMovement"/>.
    /// It also binds to the value event <see cref="EntityModel.grounded"/> (<see cref="grounded"/>, <see cref="IsGrounded"/>)
    /// </summary>
    public abstract class EntityMotor : EntityComponent
    {
        /// <summary>
        /// Determines whether or not the entity motor is currently standing on ground or in the air.
        /// </summary>
        public abstract bool grounded { get; }

        /// <summary>
        /// The interpolation factor controlling how fast leaning is being lerped.
        /// </summary>
        [Header("Leaning")]
        public float leanLerpFactor = 5;
        /// <summary>
        /// The leaning angle
        /// </summary>
        public float leanHipsAngle = 10;
        /// <summary>
        /// The hips transform.
        /// </summary>
        public Transform hips;

        /// <summary>
        /// Whether or not the entity currently wants to jump.
        /// </summary>
        [Header("Debug")]
        [SerializeField]
        protected bool wantsToJump;

        /// <summary>
        /// Whether or not the entity is currently running.
        /// </summary>
        [SerializeField]
        protected bool isRunning;

        /// <summary>
        /// The current stance of the entity.
        /// </summary>
        [SerializeField]
        protected StanceState stanceState;

        /// <summary>
        /// The current leaning state of the entity.
        /// </summary>
        [SerializeField]
        protected LeaningState leaningState;

        public override void OnRegisterEventHandlers()
        {
            // Jumping
            this.entity.model.jump.RegisterStartCondition(this.CanJump);
            this.entity.model.jump.RegisterActivityGetter(this.IsJumping);
            this.entity.model.jump.onStart += this.OnJump;

            // Running
            this.entity.model.run.RegisterActivityGetter(this.IsRunning);
            this.entity.model.run.RegisterStartCondition(this.CanRun);
            this.entity.model.run.onStart += this.OnRunStart;
            this.entity.model.run.onStop += this.OnRunStop;

            // Leaning
            this.entity.model.leanLeft.RegisterActivityGetter(this.IsLeaningLeft);
            this.entity.model.leanRight.RegisterActivityGetter(this.IsLeaningRight);
            this.entity.model.leanLeft.RegisterStartCondition(this.CanLean);
            this.entity.model.leanRight.RegisterStartCondition(this.CanLean);
            this.entity.model.leanLeft.onStart += this.OnLeanLeft;
            this.entity.model.leanRight.onStart += this.OnLeanRight;
            this.entity.model.leanLeft.onStop += this.OnLeanStop;
            this.entity.model.leanRight.onStop += this.OnLeanStop;

            // Crouching
            this.entity.model.crouch.RegisterActivityGetter(this.IsCrouching);
            this.entity.model.crouch.RegisterStartCondition(this.CanCrouch);
            this.entity.model.crouch.onStart += this.OnCrouchStart;
            this.entity.model.crouch.onStop += this.OnCrouchStop;

            // Prone
            this.entity.model.prone.RegisterActivityGetter(this.IsProne);
            this.entity.model.prone.RegisterStartCondition(this.CanProne);
            this.entity.model.prone.onStart += this.OnProneStart;
            this.entity.model.prone.onStop += this.OnProneStop;

            // Grounding check
            this.entity.model.grounded.SetGetter(this.IsGrounded);
        }

        public virtual void Update()
        {
            this.UpdateLeaning();
        }

        public virtual void FixedUpdate()
        {
            this.UpdateStance();
        }

        /// <summary>
        /// Called from <see cref="FixedUpdate"/> in order to update the stance
        /// </summary>
        protected abstract void UpdateStance();

        public bool IsGrounded()
        {
            return this.grounded;
        }

        #region Running

        private bool IsRunning()
        {
            return this.isRunning;
        }

        private bool CanRun()
        {
            return !this.isRunning && this.entity.model.motorMovement.Get().magnitude > 0.01f;
        }

        private void OnRunStart()
        {
            this.isRunning = true;
        }

        private void OnRunStop()
        {
            this.isRunning = false;
        }

        #endregion

        #region Jumping

        public bool IsJumping()
        {
            return !this.grounded || this.wantsToJump;
        }

        public bool CanJump()
        {
            return this.grounded && !this.wantsToJump;
        }

        private void OnJump()
        {
            this.wantsToJump = true;
        }

        #endregion

        #region Leaning

        /// <summary>
        /// Called from <see cref="Update"/> in order to update the leaning system.
        /// </summary>
        protected virtual void UpdateLeaning()
        {
            // Leaning
            Vector3 targetEulerLean = Vector3.zero;
            switch (this.leaningState)
            {
                case LeaningState.Left:
                    {
                        targetEulerLean.z = this.leanHipsAngle;
                    }
                    break;
                case LeaningState.Right:
                    {
                        targetEulerLean.z = -this.leanHipsAngle;
                    }
                    break;
            }

            this.hips.localRotation = Quaternion.Lerp(this.hips.localRotation, Quaternion.Euler(targetEulerLean), this.leanLerpFactor * Time.deltaTime); // Vector3.Lerp(this.hips.localEulerAngles, targetEulerLean, this.leanLerpFactor * Time.deltaTime);
        }

        // Left
        private bool IsLeaningLeft()
        {
            return this.leaningState == LeaningState.Left;
        }

        private void OnLeanLeft()
        {
            this.leaningState = LeaningState.Left;
        }

        // Right
        private bool IsLeaningRight()
        {
            return this.leaningState == LeaningState.Right;
        }

        private void OnLeanRight()
        {
            this.leaningState = LeaningState.Right;
        }

        // General
        private bool CanLean()
        {
            return this.leaningState == LeaningState.None;
        }

        private void OnLeanStop()
        {
            this.leaningState = LeaningState.None;
        }

        #endregion

        #region Crouching

        private bool IsCrouching()
        {
            return this.stanceState == StanceState.Crouch;
        }

        private bool CanCrouch()
        {
            return this.stanceState != StanceState.Crouch;
        }

        private void OnCrouchStart()
        {
            this.stanceState = StanceState.Crouch;
        }

        private void OnCrouchStop()
        {
            this.stanceState = StanceState.Stand;
        }

        #endregion

        #region Prone

        private bool IsProne()
        {
            return this.stanceState == StanceState.Prone;
        }

        private bool CanProne()
        {
            return this.stanceState != StanceState.Prone;
        }

        private void OnProneStart()
        {
            this.stanceState = StanceState.Prone;
        }

        private void OnProneStop()
        {
            this.stanceState = StanceState.Stand;
        }

        #endregion
    }
}