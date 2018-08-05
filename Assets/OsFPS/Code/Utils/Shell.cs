using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Helper script for creating shell prefabs.
    /// Will apply a specified force to the rigidbody attached to the shell on Awake.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Shell : MonoBehaviour
    {
        public new Rigidbody rigidbody;
        public Vector3 ejectionDirection;

        public float ejectionForceMin;
        public float ejectionForceMax;

        public Vector3 angularVelocityMin;
        public Vector3 angularVelocityMax;

        /// <summary>
        /// Sent from <see cref="WeaponVisualization.OnWeaponFireStart"/>
        /// </summary>
        public void InitShell()
        {
            this.rigidbody.maxAngularVelocity = float.PositiveInfinity;
            this.rigidbody.velocity = Vector3.zero;
            this.rigidbody.isKinematic = false;
            this.rigidbody.AddForce(this.transform.TransformDirection(this.ejectionDirection) * Random.Range(this.ejectionForceMin, this.ejectionForceMax), ForceMode.VelocityChange);
            this.rigidbody.angularVelocity = Vector3.Lerp(this.angularVelocityMin, this.angularVelocityMax, Random.value);
        }
    }
}