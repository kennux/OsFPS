using UnityEngine;
using System.Collections;

namespace OsFPS
{
    /// <summary>
    /// The mmost commonly used motor implementation.
    /// This implementation simulates entity movement using unity engine rigidbodies for physical integration into the world.
    /// </summary>
    public class RigidbodyMotor : EntityMotor
    {
        public override bool grounded
        {
            get
            {
                return this._grounded;
            }
        }
        private bool _grounded;

        // Speeds
        public float runningSpeed { get { return this.entity.model.movementSpeedRun.Get(); } }
        public float crouchedSpeed { get { return this.entity.model.movementSpeedCrouch.Get(); } }
        public float proneSpeed { get { return this.entity.model.movementSpeedProne.Get(); } }
        public float speed { get { return this.entity.model.movementSpeed.Get(); } }
        public float sneakSpeedFactor { get { return this.entity.model.sneakSpeedFactor.Get(); } }

        [Header("Physics")]
        public float gravity = 10.0f;
        public float maxVelocityChange = 10.0f;
        public bool canJump = true;
        public float externalForceScale = 0.025f;
        public float groundingCheckRayHeightOffset = .1f;
        public float groundingCheckRayLength = 0.2f;
        public LayerMask groundingCheckLayerMask;

        public float jumpHeight { get { return this.entity.model.jumpHeight.Get(); } }
        public float inAirControl { get { return this.entity.model.inAirControl.Get(); } }

        /// <summary>
        /// The collider that is being used for standing and crouched stances.
        /// </summary>
        [Header("Colliders")]
        public CapsuleCollider standCrouchCollider;
        public Collider proneCollider;
        public float standCrouchRadius = 0.5f;
        public float standHeight = 1.9f;
        public float crouchHeight = 1f;
        public float stanceLerpFactor = 10f;

        [Header("References")]
        public new Rigidbody rigidbody;

        public override void OnRegisterEventHandlers()
        {
            base.OnRegisterEventHandlers();

            this.entity.model.motorVelocity.SetGetter(() => this.rigidbody.velocity);
        }

        public void OnDrawGizmosSelected()
        {
            Vector3 origin, dir;
            float length;
            int layerMask;

            GetRaycastParams(out origin, out dir, out length, out layerMask);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + (dir * length));
        }

        public void Awake()
        {
            rigidbody.freezeRotation = true;
            rigidbody.useGravity = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // if (grounded)
            {
                float speed = this.speed;
                if (this.entity.model.run.IsActive())
                    speed = this.runningSpeed;
                if (this.entity.model.crouch.IsActive())
                    speed = this.crouchedSpeed;
                if (this.entity.model.prone.IsActive())
                    speed = this.proneSpeed;
                if (this.entity.model.sneak.IsActive())
                    speed *= this.sneakSpeedFactor;

                // Weapon mobility
                var weapon = this.entity.model.currentWeaponDefinition.Get();
                if (weapon != null)
                    speed *= weapon.mobility;

                // Calculate how fast we should be moving
                Vector2 targetVelocity2 = this.entity.model.motorMovement.Get();
                Vector3 targetVelocity = new Vector3(targetVelocity2.x, 0, targetVelocity2.y);
                targetVelocity = transform.TransformDirection(targetVelocity).normalized;
                targetVelocity *= speed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rigidbody.velocity;
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                rigidbody.AddForce(velocityChange * (grounded ? 1 : this.inAirControl), ForceMode.VelocityChange);

                // Jump
                if (canJump && this.wantsToJump)
                {
                    rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z) * (grounded ? 1 : this.inAirControl);
                }
                this.wantsToJump = false;
            }

            // We apply gravity manually for more tuning control
            rigidbody.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0));

            Vector3 origin, dir;
            float length;
            int layerMask;
            GetRaycastParams(out origin, out dir, out length, out layerMask);
            _grounded = Physics.Raycast(origin, dir, length, layerMask);
        }

        private void GetRaycastParams(out Vector3 origin, out Vector3 dir, out float length, out int layerMask)
        {
            origin = this.rigidbody.worldCenterOfMass + (Vector3.up * this.groundingCheckRayHeightOffset);
            dir = -this.transform.up;
            length = this.groundingCheckRayLength;
            layerMask = this.groundingCheckLayerMask;
        }

        float CalculateJumpVerticalSpeed()
        {
            float modifier = 1;
            var weapon = this.entity.model.currentWeaponDefinition.Get();
            if (weapon != null)
                modifier = weapon.mobility;

            // From the jump height and gravity we deduce the upwards speed 
            // for the character to reach at the apex.
            return Mathf.Sqrt(2 * (jumpHeight * modifier) * gravity);
        }

        protected override void UpdateStance()
        {
            switch (this.stanceState)
            {
                case StanceState.Crouch:
                case StanceState.Stand:
                    {
                        this.proneCollider.gameObject.SetActive(false);
                        this.standCrouchCollider.radius = Mathf.Lerp(this.standCrouchCollider.radius, this.standCrouchRadius, Time.deltaTime * this.stanceLerpFactor);
                        this.standCrouchCollider.height = Mathf.Lerp(this.standCrouchCollider.height, this.stanceState == StanceState.Crouch ? this.crouchHeight : this.standHeight, Time.deltaTime * this.stanceLerpFactor);
                    }
                    break;
                case StanceState.Prone:
                    {
                        this.proneCollider.gameObject.SetActive(true);
                        this.standCrouchCollider.radius = Mathf.Lerp(this.standCrouchCollider.radius, 0.01f, Time.deltaTime * this.stanceLerpFactor);
                        this.standCrouchCollider.height = Mathf.Lerp(this.standCrouchCollider.height, 0.01f, Time.deltaTime * this.stanceLerpFactor);
                    }
                    break;
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            this.entity.model.externalForce.Fire(collision.relativeVelocity * this.externalForceScale);
        }
    }
}