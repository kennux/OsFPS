using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace OsFPS
{
    /// <summary>
    /// First person weapon animation behaviour is the core of the first person weapon visualization.
    /// It provides physics based animation using <see cref="SpringPhysics"/>.
    /// </summary>
    public class FirstPersonWeaponAnimation : MonoBehaviour
    {
#if UNITY_EDITOR

        [ContextMenu("Set Hips state")]
        private void SetHipsState()
        {
            this.hipsOriginPosition = this.transform.localPosition;
            this.hipsOriginRotation = this.transform.localRotation;
        }

        [ContextMenu("Set Zoomed state")]
        private void SetZoomedState()
        {
            this.zoomedOriginPosition = this.transform.localPosition;
            this.zoomedOriginRotation = this.transform.localRotation;
        }

        [ContextMenu("Set Colliding state")]
        private void SetCollidingState()
        {
            this.collidingOriginPosition = this.transform.localPosition;
            this.collidingOriginRotation = this.transform.localRotation;
        }

        [ContextMenu("Set Running state")]
        private void SetRunningState()
        {
            this.runningOriginPosition = this.transform.localPosition;
            this.runningOriginRotation = this.transform.localRotation;
        }

        [ContextMenu("Set Prone state")]
        private void SetProneState()
        {
            this.proneOriginPosition = this.transform.localPosition;
            this.proneOriginRotation = this.transform.localRotation;
        }

        [ContextMenu("Load Hips state")]
        private void LoadHipsState()
        {
            this.transform.localPosition = this.hipsOriginPosition;
            this.transform.localRotation = this.hipsOriginRotation;
        }

        [ContextMenu("Load Zoomed state")]
        private void LoadZoomedState()
        {
            this.transform.localPosition = this.zoomedOriginPosition;
            this.transform.localRotation = this.zoomedOriginRotation;
        }

        [ContextMenu("Load Colliding state")]
        private void LoadCollidingState()
        {
            this.transform.localPosition = this.collidingOriginPosition;
            this.transform.localRotation = this.collidingOriginRotation;
        }

        [ContextMenu("Load Running state")]
        private void LoadRunningState()
        {
            this.transform.localPosition = this.runningOriginPosition;
            this.transform.localRotation = this.runningOriginRotation;
        }

        [ContextMenu("Load Prone state")]
        private void LoadProneState()
        {
            this.transform.localPosition = this.proneOriginPosition;
            this.transform.localRotation = this.proneOriginRotation;
        }

#endif
        public FirstPersonWeapon weapon
        {
            get
            {
                if (_weapon == null)
                    _weapon = GetComponent<FirstPersonWeapon>();
                return _weapon;
            }
        }
        private FirstPersonWeapon _weapon;

        [Header("Spring")]
        public Vector3 positionalStiffness;
        public Vector3 rotationalStiffness;
        public Vector3 positionalDamping;
        public Vector3 rotationalDamping;
        public Vector3 zoomedPositionalStiffness;
        public Vector3 zoomedRotationalStiffness;
        public Vector3 zoomedPositionalDamping;
        public Vector3 zoomedRotationalDamping;

        [Header("Origins")]
        [FormerlySerializedAs("hipsOrigin")]
        public Vector3 hipsOriginPosition;
        public Quaternion hipsOriginRotation;
        public Vector3 runningOriginPosition;
        public Quaternion runningOriginRotation;
        [FormerlySerializedAs("zoomedOrigin")]
        public Vector3 zoomedOriginPosition;
        public Quaternion zoomedOriginRotation;
        public Vector3 collidingOriginPosition;
        public Quaternion collidingOriginRotation;
        public Vector3 proneOriginPosition;
        public Quaternion proneOriginRotation;

        [Header("Movement animation")]
        [FormerlySerializedAs("footstepForce")]
        public Vector3 footstepForceZoomed;
        public Vector3 footstepForceHips;
        public Vector3 footstepForceRunning;
        public Vector3 footstepForceProne;

        [Header("Idle animation")]
        public float idleSwingFrequency = 1;
        public Vector3 idleSwingIntensity = new Vector3(1, 1, 0);
        public float idleSwingFrequencyZoomed = 1;
        public Vector3 idleSwingIntensityZoomed = new Vector3(1, 1, 0);

        [Header("Animation")]
        public float positionalLerpFactor = 10f;
        public float rotationalLerpFactor = 10f;

        [Header("Collision")]
        public Collider weaponCollider;

        [Header("Recoil")]
        public Vector3 recoilMin;
        public Vector3 recoilMax;
        public Vector3 recoilEulerMin;
        public Vector3 recoilEulerMax;
        public Vector2 recoilLookMin { get { return this.weapon.weaponDefinition.recoilPatternMin; } }
        public Vector2 recoilLookMax { get { return this.weapon.weaponDefinition.recoilPatternMax; } }
        public float recoilIntensityFalloff = 6f;
        public float recoilHipsIncrease = 2;
        public float recoilIntensityIncreaseMin;
        public float recoilIntensityIncreaseMax;
        private float recoilIntensity = 1;

        [Header("Sway")]
        public float swayIntensity = 10f;
        [Tooltip("XY are calculated from delta, Z is roll and read from Y")]
        public Vector3 swayEulerIntensity = new Vector3(0, 0, 4);

        [Header("Draw")]
        public Vector3 drawPositionalForce;

        // State variables:
        private Quaternion rotationLastFrame;
        private List<Collider> currentlyCollidingWith = new List<Collider>();
        private SpringPhysics positionSpring;
        private SpringPhysics eulerSpring;
        private Quaternion swayDelta = Quaternion.identity;

        public void Awake()
        {
            this.weapon.weaponFire.onStart += this.OnWeaponFire;
            this.weapon.weaponFire.RegisterStartCondition(() => !IsColliding());
            this.weapon.footstep.handler += this.OnFootstep;

            // Springs
            this.eulerSpring = new SpringPhysics(this.rotationalStiffness, this.rotationalDamping);
            this.positionSpring = new SpringPhysics(this.positionalStiffness, this.positionalDamping);
        }

        public void OnEnable()
        {
            Vector3 p;
            Quaternion q;
            GetOrigins(out p, out q);

            this.transform.localPosition = p + this.drawPositionalForce;
            this.transform.localRotation = q;
            this.positionSpring.AddForce(this.drawPositionalForce);
        }

        public void Start()
        {
            this.rotationLastFrame = Quaternion.identity;
        }

        public void FixedUpdate()
        {
            // Align collider
            this.weaponCollider.transform.position = this.transform.parent.TransformPoint(this.weapon.weaponHandler.isZoomed ? this.zoomedOriginPosition : this.hipsOriginPosition);
            this.weaponCollider.transform.rotation = this.transform.parent.rotation;

            // Determine state
            bool isRunning = this.weapon.weaponHandler == null ? false : this.weapon.weaponHandler.entity.model.run.IsActive();
            bool isZoomed = this.weapon.weaponHandler.isZoomed;
            bool isColliding = this.IsColliding();
            bool isMoving = this.weapon.weaponHandler.entity.model.motorMovement.Get().magnitude > 0.01f;

            // Spring update
            this.positionSpring.damping = isZoomed ? this.zoomedPositionalDamping : this.positionalDamping;
            this.positionSpring.stiffness = isZoomed ? this.zoomedPositionalStiffness : this.zoomedPositionalStiffness;
            this.eulerSpring.damping = isZoomed ? this.zoomedRotationalDamping : this.rotationalDamping;
            this.eulerSpring.stiffness = isZoomed ? this.zoomedRotationalStiffness : this.rotationalStiffness;
            this.positionSpring.FixedUpdate();
            this.eulerSpring.FixedUpdate();

            // Update recoil
            this.recoilIntensity = Mathf.Lerp(this.recoilIntensity, 1f, this.recoilIntensityFalloff * Time.fixedDeltaTime);

            // Read state
            Vector3 originPos;
            Quaternion originRot;
            this.GetOrigins(out originPos, out originRot);

            if (!isMoving)
            {
                // Idle anim
                Vector3 idleSwingIntensity = isZoomed ? this.idleSwingIntensityZoomed : this.idleSwingIntensity;
                float idleSwingFrequency = isZoomed ? this.idleSwingFrequencyZoomed : this.idleSwingFrequency;

                // Add up offset
                originPos += Mathf.Sin(Time.time * idleSwingFrequency) * idleSwingIntensity;
            }

            // Sway
            if (!isColliding)
            {
                originRot *= swayDelta;
            }

            // Linear interpolcation
            Quaternion rot = Quaternion.Lerp(this.transform.localRotation, originRot, this.rotationalLerpFactor * Time.fixedDeltaTime);
            this.transform.localRotation = Quaternion.Euler(rot.eulerAngles + this.eulerSpring.Get());
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, originPos, this.positionalLerpFactor * Time.fixedDeltaTime) + this.positionSpring.Get();
        }

        public void Update()
        {
            // Update sway
            Quaternion swayDelta = Quaternion.Inverse(this.rotationLastFrame) * this.transform.rotation;
            swayDelta.eulerAngles = Vector3.Scale(swayDelta.eulerAngles, this.swayEulerIntensity);
            this.swayDelta = swayDelta;

            this.rotationLastFrame = this.transform.rotation;
        }

        #region Weapon Collision

        /// <summary>
        /// Adds a collider colliding with the weapon to <see cref="currentlyCollidingWith"/>
        /// </summary>
        public void OnTriggerEnter(Collider other)
        {
            this.currentlyCollidingWith.Add(other);
        }

        /// <summary>
        /// Removes a collider colliding with the weapon from <see cref="currentlyCollidingWith"/>.
        /// </summary>
        public void OnTriggerExit(Collider other)
        {
            this.currentlyCollidingWith.Remove(other);
        }

        /// <summary>
        /// Returns whether or not the weapon is currently colliding with any other collider.
        /// Checks <see cref="currentlyCollidingWith"/> Length > 0.
        /// </summary>
        private bool IsColliding()
        {
            // We disable collisions in prone state, because the player is too close to the bottom it very often causes collisions where none should be, it also doesnt feel or look right to have them collide
            return this.weapon.weaponHandler != null && !this.weapon.weaponHandler.entity.model.prone.IsActive() && this.currentlyCollidingWith.Count > 0;
        }

        #endregion

        /// <summary>
        /// Determines current origins for the weapon spring.
        /// Will check the current state and return the appropriate position and rotation state.
        /// 
        /// Checks: isZoomed, isColliding, isRunning, isProne.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        private void GetOrigins(out Vector3 position, out Quaternion rotation)
        {
            bool isZoomed = this.weapon.weaponHandler == null ? false : this.weapon.weaponHandler.isZoomed;
            bool isColliding = this.IsColliding();
            bool isRunning = this.weapon.weaponHandler == null ? false : this.weapon.weaponHandler.entity.model.run.IsActive();
            bool isProne = this.weapon.weaponHandler == null ? false : this.weapon.weaponHandler.entity.model.prone.IsActive();

            if (isColliding)
            {
                position = this.collidingOriginPosition;
                rotation = this.collidingOriginRotation;
            }
            else if (isRunning)
            {
                position = this.runningOriginPosition;
                rotation = this.runningOriginRotation;
            }
            else if (isZoomed)
            {
                position = this.zoomedOriginPosition;
                rotation = this.zoomedOriginRotation;
            }
            else if (isProne)
            {
                position = this.proneOriginPosition;
                rotation = this.proneOriginRotation;
            }
            else
            {
                position = this.hipsOriginPosition;
                rotation = this.hipsOriginRotation;
            }
        }
        /// <summary>
        /// Applies force to the positional spring in order to simulate footsteps.
        /// </summary>
        private void OnFootstep(Foot foot)
        {
            bool isZoomed = this.weapon.weaponHandler.playerEntity.fpModel.zoom.IsActive();
            bool isRunning = this.weapon.weaponHandler.entity.model.run.IsActive();
            bool isProne = this.weapon.weaponHandler == null ? false : this.weapon.weaponHandler.entity.model.prone.IsActive();

            Vector3 v;
            if (isZoomed)
            {
                v = this.footstepForceZoomed;
            }
            else if (isRunning)
            {
                v = this.footstepForceRunning;
            }
            else if (isProne)
            {
                v = this.footstepForceProne;
            }
            else
            {
                v = this.footstepForceHips;
            }

            v.x *= foot == Foot.Left ? -1f : 1f;
            this.positionSpring.AddForce(v);
        }

        /// <summary>
        /// Applies the specified force to the animation spring physics (<see cref="positionSpring"/>).
        /// </summary>
        public void ApplyForce(Vector3 force)
        {
            this.positionSpring.AddForce(force);
        }

        private void OnWeaponFire()
        {
            float multiplier = this.weapon.weaponHandler.playerEntity.fpModel.zoom.IsActive() ? 1 : this.recoilHipsIncrease;
            Vector3 recoil = Vector3.zero;

            // Position recoil
            recoil.x = Mathf.Lerp(this.recoilMin.x, this.recoilMax.x, Random.value);
            recoil.y = Mathf.Lerp(this.recoilMin.y, this.recoilMax.y, Random.value);
            recoil.z = Mathf.Lerp(this.recoilMin.z, this.recoilMax.z, Random.value);

            this.positionSpring.AddForce(recoil * multiplier);

            // Euler recoil
            recoil.x = Mathf.Lerp(this.recoilEulerMin.x, this.recoilEulerMax.x, Random.value);
            recoil.y = Mathf.Lerp(this.recoilEulerMin.y, this.recoilEulerMax.y, Random.value);
            recoil.z = Mathf.Lerp(this.recoilEulerMin.z, this.recoilEulerMax.z, Random.value);

            this.eulerSpring.AddForce(recoil * multiplier);

            // Look recoil
            Vector2 recoil2 = new Vector2(Mathf.Lerp(this.recoilLookMin.x, this.recoilLookMax.x, Random.value), Mathf.Lerp(this.recoilLookMin.y, this.recoilLookMax.y, Random.value)) * this.weapon.weaponDefinition.recoil;
            this.weapon.weaponHandler.playerEntity.fpModel.cameraRecoil.Fire(recoil2 * this.recoilIntensity * multiplier);

            // Increase recoil intensity
            this.recoilIntensity += Mathf.Lerp(this.recoilIntensityIncreaseMin, this.recoilIntensityIncreaseMax, Random.value);
        }
    }
}