using UnityEngine;
using System.Collections;

namespace OsFPS
{
    /// <summary>
    /// Enumeration for <see cref="EntityMotor"/> leaning states.
    /// </summary>
    public enum LeaningState
    {
        /// <summary>
        /// No leaning at all
        /// </summary>
        None,

        /// <summary>
        /// Currently leaning towards left.
        /// </summary>
        Left,

        /// <summary>
        /// Currently leaning towards right.
        /// </summary>
        Right
    }
}