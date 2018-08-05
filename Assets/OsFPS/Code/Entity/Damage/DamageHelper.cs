using UnityEngine;
using System.Collections;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Helper class for interfering with the damage system.
    /// </summary>
    public static class DamageHelper
    {
        /// <summary>
        /// Helper method which can send damage to a specific gameobject.
        /// It first gets the closest parent that has a damage handler (<see cref="IDamageHandler"/>) and then all other damage handlers on that gameobject.
        /// </summary>
        public static void SendDamage(this GameObject gameObject, DamageEventArgs args)
        {
            // Try spawning debris
            DebrisManager.instance.TrySpawnDebris(gameObject, args.hitPoint, args.hitNormal, DebrisSpawn.BulletDecal | DebrisSpawn.DebrisParticles);

            // Get damagehandle in parents
            var dh = gameObject.GetComponentInParent<IDamageHandler>();

            // Does have a damage handler=
            if (dh != null)
            {
                // Read all damage handlers in case there are multiple on the handler object.
                var dhs = ListPool<IDamageHandler>.Get();
                dh.gameObject.GetComponents(dhs);

                // Send damage to all handlers
                for (int i = 0; i < dhs.Count; i++)
                    dhs[i].TakeDamage(args);

                ListPool<IDamageHandler>.Return(dhs);
            }
        }
    }
}