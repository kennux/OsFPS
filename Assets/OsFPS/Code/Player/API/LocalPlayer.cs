using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK;

namespace OsFPS
{
    /// <summary>
    /// Static interface class to access the local player.
    /// This only is to provide a few shortcuts to commonly needed tasks and its essentially just a simplification proxy for convenience.
    /// </summary>
    public static class LocalPlayer
    {
        public static FirstPersonEntity player { get { return UnitySingleton<FirstPersonEntity>.Get(); } }

        public static float health { get { return player.model.health.Get(); } set { player.model.health.Set(value); } }
        public static float maxHealth { get { return player.model.maxHealth.Get(); } }

        public static void Teleport(Vector3 position)
        {
            player.transform.position = position;
        }

        public static void GiveWeapon(WeaponDefinition weapon, int ammo)
        {
            player.model.pickupWeapon.Try(new WeaponAmmoTuple(weapon, ammo));
        }

        public static bool HasWeaponEquiped()
        {
            return player.model.currentWeaponDefinition.Get() != null;
        }

        public static WeaponDefinition GetCurrentWeaponDefinition()
        {
            return player.model.currentWeaponDefinition.Get();
        }

        public static Weapon GetCurrentWeapon()
        {
            return player.model.currentWeapon.Get();
        }


        public static int GetCurrentWeaponAmmo()
        {
            var weapon = GetCurrentWeapon();
            if (weapon == null)
                return 0;

            return weapon.ammo;
        }

        public static int GetCurrentWeaponAmmoInClip()
        {
            var weapon = GetCurrentWeapon();
            if (weapon == null)
                return 0;

            return weapon.ammoInClip;
        }

    }
}