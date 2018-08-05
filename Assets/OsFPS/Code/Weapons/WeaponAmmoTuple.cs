using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsFPS
{
    /// <summary>
    /// A tuple of weapon definition and ammo.
    /// This is used to transfer information about a weapon pick up for example.
    /// </summary>
    public struct WeaponAmmoTuple
    {
        /// <summary>
        /// The weapon definition.
        /// </summary>
        public WeaponDefinition weapon;

        /// <summary>
        /// The amount of ammo.
        /// </summary>
        public int ammo;

        public WeaponAmmoTuple(WeaponDefinition weapon, int ammo)
        {
            this.weapon = weapon;
            this.ammo = ammo;
        }
    }
}
