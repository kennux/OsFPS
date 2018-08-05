using UnityEngine;
using System.Collections;

namespace OsFPS
{
    /// <summary>
    /// Parameters for an interaction (<see cref="IInteractable"/>).
    /// </summary>
    [System.Serializable]
    public class InteractionParameters
    {
        /// <summary>
        /// The amount of time it takes to interact with the object.
        /// </summary>
        public float duration;
    }
}