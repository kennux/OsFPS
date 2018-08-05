using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Specialized entity implementation that is used for local players (first person players).
    /// </summary>
    [RequireComponent(typeof(FirstPersonEntity))]
    public class FirstPersonEntity : Entity
    {
        /// <summary>
        /// The first person controller of this entity.
        /// </summary>
        public FirstPersonController fpController { get { return this.controller as FirstPersonController; } }

        /// <summary>
        /// The specialized entity model for FP.
        /// </summary>
        public FirstPersonPlayerModel fpModel { get { return this.model as FirstPersonPlayerModel; } }

        public void Awake()
        {
            UnitySingleton<FirstPersonEntity>.Register(this);
        }
    }
}