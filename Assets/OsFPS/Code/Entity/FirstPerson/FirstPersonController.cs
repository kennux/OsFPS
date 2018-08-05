using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// First person controller that takes player input and sends appropriate entity events.
    /// </summary>
    [RequireComponent(typeof(FirstPersonEntity))]
    public class FirstPersonController : EntityController
    {
        /// <summary>
        /// The first person entity, where the input is applied to.
        /// </summary>
        public FirstPersonEntity fpEntity
        {
            get
            {
                if (this._fpEntity == null)
                    this._fpEntity = GetComponent<FirstPersonEntity>();
                return this._fpEntity;
            }
        }
        private FirstPersonEntity _fpEntity;

        /// <summary>
        /// Used for interaction raycasting.
        /// </summary>
        public Camera fpsCamera;

        /// <summary>
        /// The layermask for raycasting for interactables.
        /// </summary>
        public LayerMask interactionRaycastMask;

        /// <summary>
        /// The maximum interaction distance.
        /// </summary>
        public float interactionDistance = 3f;

        public void Update()
        {
            bool wantsToLeanLeft = Input.GetButton("LeanLeft");
            bool wantsToLeanRight = Input.GetButton("LeanRight");
            bool isLeaningLeft = this.fpEntity.model.leanLeft.IsActive();
            bool isLeaningRight = this.fpEntity.model.leanRight.IsActive();
            bool isLeaning = isLeaningLeft || isLeaningRight;
            bool isNotLeaning = !isLeaning;

            // Determine reticle state
            bool noWeaponOrZooming = this.entity.model.currentWeapon.Get() == null || this.fpEntity.fpModel.zoom.IsActive();
            HUD.instance.reticleState = noWeaponOrZooming ? ReticleState.None : ReticleState.Crosshair;

            // Interaction raycast
            var interactionRay = this.fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit interactionHit;
            if (Physics.Raycast(interactionRay, out interactionHit, this.interactionDistance, this.interactionRaycastMask))
            {
                IInteractable interactable = interactionHit.collider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    if (Input.GetButtonDown("Interact"))
                        this.entity.model.interact.TryStart(interactable);

                    HUD.instance.reticleState = ReticleState.Interaction;
                }
            }

            // Reticle positioning
            if (HUD.instance.reticleState == ReticleState.Crosshair)
                HUD.instance.SetReticlePosition(this.fpsCamera.WorldToScreenPoint(this.entity.model.projectileOrigin.Get() + (this.entity.model.projectileOriginDir.Get() * 2.5f)));
            else
                HUD.instance.SetReticlePosition(new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (wantsToLeanLeft)
            {
                if (isNotLeaning)
                    this.fpEntity.model.leanLeft.TryStart();
            }
            else if (isLeaningLeft)
                this.fpEntity.model.leanLeft.TryStop();

            if (wantsToLeanRight)
            {
                if (isNotLeaning)
                    this.fpEntity.model.leanRight.TryStart();
            }
            else if (isLeaningRight)
                this.fpEntity.model.leanRight.TryStop();


            if (Input.GetButton("Run"))
            {
                if (!this.fpEntity.model.run.IsActive())
                    this.fpEntity.model.run.TryStart();
            }
            else
            {
                if (this.fpEntity.model.run.IsActive())
                    this.fpEntity.model.run.TryStop();
            }

            if (Input.GetButtonDown("Jump"))
                this.entity.model.jump.TryStart();

            if (Input.GetButtonDown("Fire"))
                this.fpEntity.fpModel.fire.TryStart();
            if (Input.GetButtonUp("Fire"))
                this.fpEntity.fpModel.fire.TryStop();

            if (Input.GetButtonDown("ChangeFireMode"))
            {
                var fm = this.entity.model.fireMode.Get();
                var fms = this.entity.model.availableFireModes.Get();
                int index = System.Array.IndexOf(fms, fm) + 1;

                if (index >= fms.Length)
                    index = 0;

                this.entity.model.setFireMode.Try(fms[index]);
            }

            if (Input.GetButtonDown("Zoom"))
            {
                if (this.fpEntity.fpModel.zoom.IsActive())
                    this.fpEntity.fpModel.zoom.TryStop();
                else
                    this.fpEntity.fpModel.zoom.TryStart();
            }

            if (Input.GetButtonDown("Crouch"))
            {
                if (this.fpEntity.fpModel.crouch.IsActive())
                    this.fpEntity.fpModel.crouch.TryStop();
                else
                    this.fpEntity.fpModel.crouch.TryStart();
            }

            if (Input.GetButtonDown("Prone"))
            {
                if (this.fpEntity.fpModel.prone.IsActive())
                    this.fpEntity.fpModel.prone.TryStop();
                else
                    this.fpEntity.fpModel.prone.TryStart();
            }

            if (Input.GetButtonDown("Sneak"))
            {
                if (this.fpEntity.fpModel.sneak.IsActive())
                    this.fpEntity.fpModel.sneak.TryStop();
                else
                    this.fpEntity.fpModel.sneak.TryStart();
            }

            if (Input.GetButtonDown("Holster"))
            {
                this.fpEntity.fpModel.holsterWeapon.Fire();
            }

            if (Input.GetButtonDown("Reload"))
                this.entity.model.reload.TryStart();

            if (Input.GetButtonDown("Weapon1"))
                this.entity.model.selectWeaponByIndex.Try(0);
            if (Input.GetButtonDown("Weapon2"))
                this.entity.model.selectWeaponByIndex.Try(1);
            if (Input.GetButtonDown("Weapon3"))
                this.entity.model.selectWeaponByIndex.Try(2);
            if (Input.GetButtonDown("Weapon4"))
                this.entity.model.selectWeaponByIndex.Try(3);
        }

        /// <summary>
        /// Getter of <see cref="EntityModel.motorMovement"/>
        /// </summary>
        private Vector2 GetMovement()
        {
            var v = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            return v;
        }

        /// <summary>
        /// Getter of <see cref="FirstPersonPlayerModel.inputLook"/>
        /// </summary>
        private Vector2 GetCameraMovement()
        {
            Vector2 cameraMovement = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            return cameraMovement;
        }

        public override void OnRegisterEventHandlers()
        {
            this.entity.model.motorMovement.SetGetter(this.GetMovement);
            this.fpEntity.fpModel.inputLook.SetGetter(this.GetCameraMovement);
        }
    }
}