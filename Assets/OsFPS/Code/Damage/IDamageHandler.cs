using UnityEngine;
using System.Collections;

namespace OsFPS
{
    /// <summary>
    /// Damage handler interface, provides basic methods for interacting with objects that can take damage in some way.
    /// </summary>
    public interface IDamageHandler
    {
        /// <summary>
        /// The gameobject this damage handler is living on.
        /// </summary>
        GameObject gameObject { get; }

        /// <summary>
        /// Called when this handler took damage.
        /// </summary>
        void TakeDamage(DamageEventArgs args);
    }
}