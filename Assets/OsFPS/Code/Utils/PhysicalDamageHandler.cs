using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Damage handler implementation that can be used to translate damage onto a rigidbody as force.
    /// Uses <see cref="DamageEventArgs.hitNormal"/>, <see cref="DamageEventArgs.physicalForce"/> and <see cref="DamageEventArgs.hitPoint"/>.
    /// </summary>
    public class PhysicalDamageHandler : MonoBehaviour, IDamageHandler
    {
        public Rigidbody reciever;

        [ContextMenu("Auto assign reciever")]
        private void AutoAssignReciever()
        {
            this.reciever = this.GetComponent<Rigidbody>();
        }

        public void TakeDamage(DamageEventArgs args)
        {
            this.reciever.AddForceAtPosition(args.hitNormal * -1f * args.physicalForce, args.hitPoint);
        }
    }
}