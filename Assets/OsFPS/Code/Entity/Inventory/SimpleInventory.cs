using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsFPS
{
    /// <summary>
    /// Simple inventory implementation.
    /// </summary>
    public class SimpleInventory : EntityComponent
    {
        /// <summary>
        /// Inventory data.
        /// Key = Weapon,
        /// Value = ammo
        /// </summary>
        private Dictionary<WeaponDefinition, int> inventory = new Dictionary<WeaponDefinition, int>();

        /// <summary>
        /// The maximum amount of weapons this inventory can store.
        /// </summary>
        public int maxAmountWeapons = 2;
        
        public override void OnRegisterEventHandlers()
        {
            this.entity.model.getWeaponAmmo.BindHandler(this.GetAmmo);
            this.entity.model.setWeaponAmmo.handler += this.SetAmmo;
            this.entity.model.availableWeapons.RegisterGetter(GetWeapons);

            this.entity.model.pickupWeapon.RegisterCondition(this.CanPickupWeapon);
            this.entity.model.pickupWeapon.onFire += this.OnPickupWeapon;

            this.entity.model.dropWeapon.RegisterCondition(this.CanDropWeapon);
            this.entity.model.dropWeapon.onFire += this.OnDropWeapon;
        }

        /// <summary>
        /// Directly sets ammuniation amount.
        /// </summary>
        public virtual void SetAmmo(WeaponDefinition weapon, int ammo)
        {
            if (this.inventory.ContainsKey(weapon))
                this.inventory[weapon] = ammo;
        }

        /// <summary>
        /// Condition for <see cref="EntityModel.dropWeapon"/>
        /// </summary>
        /// <param name="weaponDefinition">The weapon definition to drop.</param>
        /// <returns>Whether or not the weapon can be dropped.</returns>
        public virtual bool CanDropWeapon(WeaponDefinition weaponDefinition)
        {
            return this.inventory.ContainsKey(weaponDefinition);
        }

        /// <summary>
        /// Handler for <see cref="EntityModel.dropWeapon"/>
        /// </summary>
        /// <param name="weaponDefinition">The weapon to drop.</param>
        public virtual void OnDropWeapon(WeaponDefinition weaponDefinition)
        {
            if (this.inventory.ContainsKey(weaponDefinition))
            {
                this.inventory.Remove(weaponDefinition);
                this.entity.model.onDroppedWeapon.Fire(weaponDefinition);
            }
        }

        /// <summary>
        /// Whether or not a weapon can be picked up.
        /// <see cref="EntityModel.pickupWeapon"/>
        /// </summary>
        /// <param name="weaponAmmo">Weapon and ammo to be picked up.</param>
        public virtual bool CanPickupWeapon(WeaponAmmoTuple weaponAmmo)
        {
            if (this.inventory.Count >= this.maxAmountWeapons)
                return false;

            return !this.inventory.ContainsKey(weaponAmmo.weapon);
        }

        /// <summary>
        /// Handler for picking up a weapon with ammo.
        /// <see cref="EntityModel.pickupWeapon"/>
        /// </summary>
        public virtual void OnPickupWeapon(WeaponAmmoTuple weaponAmmo)
        {
            int curAmmo = 0;
            if (this.inventory.TryGetValue(weaponAmmo.weapon, out curAmmo))
                this.inventory[weaponAmmo.weapon] = curAmmo + weaponAmmo.ammo;
            else
            {
                this.entity.model.onPickedupWeapon.Fire(weaponAmmo.weapon);
                this.inventory.Add(weaponAmmo.weapon, weaponAmmo.ammo);
            }

        }

        protected virtual ICollection<WeaponDefinition> GetWeapons()
        {
            return this.inventory.Keys;
        }

        protected virtual int GetAmmo(WeaponDefinition weapon)
        {
            int ammo = 0;
            this.inventory.TryGetValue(weapon, out ammo);
            return ammo;
        }
    }
}
