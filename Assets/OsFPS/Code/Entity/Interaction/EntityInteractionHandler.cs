using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Interaction handler for entities.
    /// 
    /// Handles entity -> world interaction (<see cref="IInteractable"/>).
    /// Binds to <see cref="EntityModel.interact"/> and <see cref="EntityModel.interactionProgress"/>
    /// </summary>
    public class EntityInteractionHandler : EntityComponent
    {
        /// <summary>
        /// The maximum iteraction distance.
        /// </summary>
        [Header("Config")]
        public float interactDistance;

        /// <summary>
        /// The transform whose position is used to determine whether the entity can interact with something (<see cref="interactDistance"/>).
        /// </summary>
        public Transform interactionOrigin;

        /// <summary>
        /// The time when the currently ongoing interaction will be completed.
        /// </summary>
        [Header("Debug")]
        [SerializeField]
        private float interactionDone;
        /// <summary>
        /// The time at which the current interaction started.
        /// </summary>
        [SerializeField]
        private float interactionStartTime;

        /// <summary>
        /// The interactable this object is currently interacting with.
        /// </summary>
        private IInteractable currentInteractable;

        public void Awake()
        {
            this.currentInteractable = null;
            this.interactionDone = -1;
            this.interactionStartTime = -1;
        }

        public void Update()
        {
            if (object.ReferenceEquals(this.currentInteractable, null))
                return;

            if (!CanInteract(this.currentInteractable))
            {
                // Invalidated?
                this.entity.model.interact.ForceStop();
                return;
            }

            // Interaction over?
            if (this.interactionDone <= Time.time)
            {
                this.interactionDone = -1;
                this.interactionStartTime = -1;
                this.currentInteractable.OnInteractionFinished(this.entity);
                this.currentInteractable = null;
            }
        }

        public override void OnRegisterEventHandlers()
        {
            this.entity.model.interact.RegisterActivityGetter(this.IsInteracting);
            this.entity.model.interact.RegisterStartCondition(this.CanInteract);
            this.entity.model.interact.onStart += this.OnStartInteract;
            this.entity.model.interact.onStop += this.OnStopInteract;
        }

        /// <summary>
        /// Returns the current interaction progress.
        /// </summary>
        private float GetInteractionProgress()
        {
            if (object.ReferenceEquals(this.currentInteractable, null))
                return 0;
            return (Time.time - this.interactionStartTime) / (this.interactionDone - this.interactionStartTime);
        }

        /// <summary>
        /// Activity-Getter of <see cref="EntityModel.interact"/>
        /// </summary>
        private bool IsInteracting()
        {
            // Currently interacting?
            return !object.ReferenceEquals(this.currentInteractable, null);
        }

        /// <summary>
        /// Condition for <see cref="EntityModel.interact"/>
        /// </summary>
        private bool CanInteract(IInteractable interactable)
        {
            // Distance check
            return Vector3.Distance(this.interactionOrigin.position, interactable.collider.ClosestPoint(this.interactionOrigin.position)) <= this.interactDistance;
        }

        /// <summary>
        /// Start handler of <see cref="EntityModel.interact"/>
        /// </summary>
        private void OnStartInteract(IInteractable interactable)
        {
            if (interactable.parameters.duration == 0)
            {
                // instant interact
                interactable.OnInteractionStarted(this.entity);
                interactable.OnInteractionCanceled(this.entity);
                interactable.OnInteractionFinished(this.entity);
                return;
            }

            // Start interaction
            this.currentInteractable = interactable;
            this.currentInteractable.OnInteractionStarted(this.entity);
            this.interactionDone = Time.time + interactable.parameters.duration;
            this.interactionStartTime = Time.time;
        }

        /// <summary>
        /// Stop handler of <see cref="EntityModel.interact"/>
        /// </summary>
        private void OnStopInteract()
        {
            // Interrupt
            if (!object.ReferenceEquals(this.currentInteractable, null))
                this.currentInteractable.OnInteractionCanceled(this.entity);
        }

    }
}