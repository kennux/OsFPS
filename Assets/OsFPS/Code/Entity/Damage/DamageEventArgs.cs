using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Arguments structure for damage events handled by <see cref="IDamageHandler"/>.
    /// </summary>
    public struct DamageEventArgs
    {
        /// <summary>
        /// Construction helper for damage events.
        /// </summary>
        /// <param name="damage">The amount of damage this event will inflict.</param>
        /// <param name="hitPoint">The point in worldspace where the damage will be inflicted.</param>
        /// <param name="hitNormal">The normal of the ray, this is a vector poiting from the hit point to the hit origin in worldspace.</param>
        /// <param name="physicalForce">The amount of physical force this event will inflict.</param>
        /// <param name="createdByMechanics">Whether or not this even was created by game mechanics, <see cref="createdByMechanics"/></param>
        /// <returns>Damage event data structure.</returns>
        public static DamageEventArgs Create(float damage, Vector3 hitPoint, Vector3 hitNormal, float physicalForce, bool createdByMechanics = false)
        {
            return new DamageEventArgs()
            {
                damage = damage,
                hitNormal = hitNormal,
                hitPoint = hitPoint,
                physicalForce = physicalForce,
                createdByMechanics = createdByMechanics
            };
        }

        /// <summary>
        /// The worldspace point at which the damage was inflicted.
        /// </summary>
        public Vector3 hitPoint;

        /// <summary>
        /// The normal of <see cref="hitPoint"/> (i.e. the direction the hit was inflicted from).
        /// </summary>
        public Vector3 hitNormal;

        /// <summary>
        /// The amount of damage that was dealt.
        /// </summary>
        public float damage;

        /// <summary>
        /// The physical force of the damage
        /// </summary>
        public float physicalForce;

        /// <summary>
        /// Whether or not this damage event argument was created by a game mechanic except shooting or other intentional events that cause damage.
        /// This is most likely used in game logic in order to apply some damage without any specific damage post processing.
        /// </summary>
        public bool createdByMechanics;
    }
}