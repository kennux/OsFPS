using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// First person look controller.
    /// This component essentially handles the first person perspective.
    /// 
    /// Handles:
    /// - Stances (Stand, Crouch, Prone)
    /// - Leaning (Left, Right, None)
    /// </summary>
    [RequireComponent(typeof(FirstPersonEntity))]
    public class FirstPersonLook : GenericEntityComponent<FirstPersonEntity>
    {
        /// <summary>
        /// The main camera of the first person player
        /// </summary>
        [Header("References")]
        public new Camera camera;

        /// <summary>
        /// The weapon camera.
        /// </summary>
        public Camera weaponCamera;

        /// <summary>
        /// The minimum rotation of the camera (euler x-angle).
        /// This controls the limits of looking up/down.
        /// </summary>
        [Header("Camera")]
        public float minRotation = -45;

        /// <summary>
        /// The maximum rotation of the camera (euler x-angle).
        /// This controls the limits of looking up/down.
        /// </summary>
        public float maxRotation = 45;

        /// <summary>
        /// The field of view of the camera in normal state.
        /// </summary>
        public float fov = 75;

        /// <summary>
        /// Camera field of view when zoming.
        /// </summary>
        public float zoomFov = 45;

        /// <summary>
        /// The factor controlling how fast fov state will be interpolated (changed).
        /// </summary>
        public float fovLerpFactor = 25f;

        /// <summary>
        /// Footstep bobbing direction on euler angles.
        /// Applied as force to <see cref="eulerSpring"/>
        /// </summary>
        public Vector2 footstepBobbing;

        /// <summary>
        /// The stiffness of the <see cref="eulerSpring"/>
        /// </summary>
        [Header("Spring")]
        public Vector3 rotationalStiffness;

        /// <summary>
        /// The damping of <see cref="eulerSpring"/>
        /// </summary>
        public Vector3 rotationalDamping;

        /// <summary>
        /// Spring physics used to simulate camera euler.
        /// </summary>
        private SpringPhysics eulerSpring;

        /// <summary>
        /// Internal camera pitch value (euler x-angle).
        /// </summary>
        private float pitch;

        public override void Awake()
        {
            base.Awake();

            this.eulerSpring = new SpringPhysics(this.rotationalStiffness, this.rotationalDamping);
        }

        public override void OnRegisterEventHandlers()
        {
            this.player.model.lookDir.SetGetter(this.GetLookDir);
            this.player.model.lookOrigin.SetGetter(this.GetLookOrigin);

            this.player.fpModel.cameraRecoil.handler += this.OnRecoil;
            this.player.fpModel.footstep.handler += this.OnFootstep;
        }

        public void FixedUpdate()
        {
            this.eulerSpring.FixedUpdate();
        }

        public void Update()
        {
            var euler = this.camera.transform.localEulerAngles;
            pitch = euler.x = Mathf.Clamp((pitch - this.player.fpModel.inputLook.Get().y), this.minRotation, this.maxRotation);
            euler += this.eulerSpring.Get();
            this.camera.transform.localEulerAngles = euler;

            // Rotation
            euler = this.transform.localEulerAngles;
            euler.y += this.player.fpModel.inputLook.Get().x;
            euler.y = euler.y < -360 ? euler.y + 360 : euler.y;
            euler.y = euler.y > 360 ? euler.y - 360 : euler.y;
            this.transform.localEulerAngles = euler;

            // FoV
            bool isZoomed = this.player.fpModel.zoom.IsActive();
            this.weaponCamera.fieldOfView = this.camera.fieldOfView = Mathf.Lerp(this.camera.fieldOfView, isZoomed ? this.zoomFov : this.fov, this.fovLerpFactor * Time.deltaTime);
        }

        /// <summary>
        /// Handler for <see cref="FirstPersonPlayerModel"/>
        /// </summary>
        private void OnFootstep(Foot foot)
        {
            float dir = foot == Foot.Left ? -1 : 1;
            this.eulerSpring.AddForce(new Vector3(-this.footstepBobbing.x, this.footstepBobbing.y * dir, 0));
        }

        /// <summary>
        /// Returns the eye look direction, bound to <see cref="EntityModel.lookDir"/>
        /// </summary>
        private Vector3 GetLookDir()
        {
            return this.camera.transform.forward;
        }

        /// <summary>
        /// Gets the eye position, bound to <see cref="EntityModel.lookOrigin"/>
        /// </summary>
        /// <returns></returns>
        private Vector3 GetLookOrigin()
        {
            return this.camera.transform.position;
        }

        /// <summary>
        /// Processes camera recoil from weapons, bound to <see cref="FirstPersonPlayerModel.cameraRecoil"/>.
        /// </summary>
        private void OnRecoil(Vector2 v)
        {
            this.pitch = Mathf.Clamp(this.pitch + v.x, this.minRotation, this.maxRotation);

            var euler = this.transform.localEulerAngles;
            euler.y += v.y;
            euler.y = euler.y < -360 ? euler.y + 360 : euler.y;
            euler.y = euler.y > 360 ? euler.y - 360 : euler.y;
            this.transform.localEulerAngles = euler;
        }
    }
}