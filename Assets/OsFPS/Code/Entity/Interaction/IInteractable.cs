using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// This interface defines an api for entities interacting with world objects!
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// The collider this interactable has attached.
        /// </summary>
        Collider collider { get; }

        /// <summary>
        /// Returns the parameters of the interaction this interactable creates.
        /// </summary>
        InteractionParameters parameters { get; }

        /// <summary>
        /// Called when the interaction with this object wwas started by an entity.
        /// </summary>
        void OnInteractionStarted(Entity user);

        /// <summary>
        /// Called when the interaction was started (<see cref="OnInteractionStarted(Entity)"/> was called), but the entity interrupted it.
        /// </summary>
        /// <param name="user"></param>
        void OnInteractionCanceled(Entity user);

        /// <summary>
        /// Called when an entity uses this interactable.
        /// Called after the interaction was finished.
        /// </summary>
        /// <param name="user"></param>
        void OnInteractionFinished(Entity user);
    }
}