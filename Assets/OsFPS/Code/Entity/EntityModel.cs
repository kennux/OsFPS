using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK.BehaviourModel;

namespace OsFPS
{
    public enum Foot : byte { Left, Right }
    /// <summary>
    /// Core component of the entity system.
    /// An entity model defines:
    /// - The activities an entity can engage in
    /// - The things it can attempt to do
    /// - Other events it can create
    /// 
    /// Those are implemented via an event-driven structure. The entity model defines the events and the component registers / react to them or fire them.
    /// Apart from that the entity model can also define several properties of the entity like a moving speed for example.
    /// 
    /// Overall the entity model models the entity behaviour in a very abstract form that is implemented by its components.
    /// </summary>
    public class EntityModel : BehaviourModel
    {
        #region Values

        /// <summary>
        /// The movement speed in units/s²
        /// </summary>
        [Header("Movement")]
        public ModelModifiableFloat movementSpeed;

        /// <summary>
        /// The movement speed coruched in units/s²
        /// </summary>
        public ModelModifiableFloat movementSpeedCrouch;

        /// <summary>
        /// The movement speed running in units/s²
        /// </summary>
        public ModelModifiableFloat movementSpeedRun;

        /// <summary>
        /// The movement speed prone in units/s²
        /// </summary>
        public ModelModifiableFloat movementSpeedProne;

        /// <summary>
        /// The movement speed factor of sneaking.
        /// </summary>
        public ModelModifiableFloat sneakSpeedFactor;

        /// <summary>
        /// The jump height in units.
        /// </summary>
        public ModelModifiableFloat jumpHeight;

        /// <summary>
        /// In air control in 0-1 range.
        /// </summary>
        public ModelModifiableFloat inAirControl;

        /// <summary>
        /// The maximum amount of health.
        /// </summary>
        [Header("Health")]
        public ModelModifiableFloat maxHealth = new ModelModifiableFloat(100);

        /// <summary>
        /// Multiplicator for incoming (taken) damage.
        /// </summary>
        public ModelModifiableFloat damageTaken = new ModelModifiableFloat(1);

        #endregion

        #region General
        /// <summary>
        /// The dir the player is currently looking towards (relative to <see cref="lookOrigin"/>).
        /// </summary>
        public ModelProperty<Vector3> lookDir = new ModelProperty<Vector3>();

        /// <summary>
        /// The look origin, in first person view this is the eye position.
        /// </summary>
        public ModelProperty<Vector3> lookOrigin = new ModelProperty<Vector3>();

        /// <summary>
        /// The origin position (in worldspace) where bullets will be fired from.
        /// </summary>
        public ModelProperty<Vector3> projectileOrigin = new ModelProperty<Vector3>();

        /// <summary>
        /// The direction projectiles will be fired from in relation to <see cref="projectileOrigin"/>.
        /// </summary>
        public ModelProperty<Vector3> projectileOriginDir = new ModelProperty<Vector3>();

        /// <summary>
        /// Whether or not this entity is currently grounded.
        /// </summary>
        public ModelProperty<bool> grounded = new ModelProperty<bool>();

        /// <summary>
        /// The current fire mode, <see cref="FireMode.NULL"/> if no weapon is selected.
        /// </summary>
        public ModelProperty<FireMode> fireMode = new ModelProperty<FireMode>();

        /// <summary>
        /// All available fire modes, usually new FireMode[0] if no weapon is selected.
        /// </summary>
        public ModelProperty<FireMode[]> availableFireModes = new ModelProperty<FireMode[]>();

        /// <summary>
        /// The interaction progress in 0-1 range.
        /// </summary>
        public ModelProperty<float> interactionProgress = new ModelProperty<float>();
        #endregion

        #region Motor
        /// <summary>
        /// Movement input event can be used to provide 2-dimensional movement vectors to a rigidbody or similar entity motor.
        /// This is used in <see cref="RigidbodyMotor"/>
        /// </summary>
        public ModelProperty<Vector2> motorMovement = new ModelProperty<Vector2>();

        /// <summary>
        /// The current motor velocity.
        /// </summary>
        public ModelProperty<Vector3> motorVelocity = new ModelProperty<Vector3>();

        /// <summary>
        /// Called on every footstep.
        /// </summary>
        public ModelEvent<Foot> footstep = new ModelEvent<Foot>();

        /// <summary>
        /// Called from the entity motor if implemented in order to make other components aware of an external force being applied to the player.
        /// This is for example called whenever the colider collides with something else in order to simulate the impact in the weapon animation.
        /// </summary>
        public ModelEvent<Vector3> externalForce = new ModelEvent<Vector3>();
        #endregion

        #region Damage
        /// <summary>
        /// The current health of the entity in health points.
        /// </summary>
        public ModelProperty<float> health = new ModelProperty<float>();

        /// <summary>
        /// Sent from a damage handler when damage was taken.
        /// </summary>
        public ModelEvent<DamageEventArgs> onDamageTaken = new ModelEvent<DamageEventArgs>();
        #endregion

        #region Weapons
        /// <summary>
        /// The weapon the entity currently has equipped / selected.
        /// </summary>
        public ModelProperty<WeaponDefinition> currentWeaponDefinition = new ModelProperty<WeaponDefinition>();

        /// <summary>
        /// The current weapon instance.
        /// </summary>
        public ModelProperty<Weapon> currentWeapon = new ModelProperty<Weapon>();

        /// <summary>
        /// Attempts to equip / select the specified weapon.
        /// </summary>
        public ModelAttempt<WeaponDefinition> selectWeapon = new ModelAttempt<WeaponDefinition>();

        /// <summary>
        /// <see cref="selectWeapon"/> by index in <see cref="availableWeapons"/>.
        /// </summary>
        public ModelAttempt<int> selectWeaponByIndex = new ModelAttempt<int>();

        /// <summary>
        /// Tries to set the fire mode of the currently equipped / selected weapon.
        /// </summary>
        public ModelAttempt<FireMode> setFireMode = new ModelAttempt<FireMode>();
        #endregion

        #region Activities
        /// <summary>
        /// Started when the entity intents to start firing, stopped either automatically on semi-fire mode or out of ammo on full auto.
        /// Also stopped as soon as the entity intents to stop firing.
        /// </summary>
        public ModelActivity fire = new ModelActivity();

        /// <summary>
        /// Started when the entity intents to reload the weapon.
        /// </summary>
        public ModelActivity reload = new ModelActivity();

        /// <summary>
        /// Fired from a health / damage handler when the entity dies due to hp falling down to 0.
        /// </summary>
        public ModelActivity death = new ModelActivity();

        /// <summary>
        /// Started when the entity intents to jump, activity getter should essentially be a grounding flag.
        /// </summary>
        public ModelActivity jump = new ModelActivity();

        /// <summary>
        /// Started when the entity intents to lean to the left, mutually exclusive to <see cref="leanRight"/>
        /// </summary>
        public ModelActivity leanLeft = new ModelActivity();

        /// <summary>
        /// Started when the entity intents to lean to the right, mutually exclusive to <see cref="leanLeft"/>
        /// </summary>
        public ModelActivity leanRight = new ModelActivity();

        /// <summary>
        /// Started when the entity intents to crouch, stopped when it intents to get back to normal stance.
        /// </summary>
        public ModelActivity crouch = new ModelActivity();

        /// <summary>
        /// Started when the entity intents to start run.
        /// </summary>
        public ModelActivity run = new ModelActivity();

        /// <summary>
        /// Started when the entity intents to prone, stopped when it intents to get back to normal stance.
        /// </summary>
        public ModelActivity prone = new ModelActivity();

        /// <summary>
        /// Started when the entity intents to sneak, in general it can be expected <see cref="footstep"/> isnt being called in sneak mode, aswell as the movement speed being lowered.
        /// </summary>
        public ModelActivity sneak = new ModelActivity();

        /// <summary>
        /// Called in order to interact with an object in the world.
        /// This is implemented by <see cref="EntityInteractionHandler"/>
        /// </summary>
        public ModelActivity<IInteractable> interact = new ModelActivity<IInteractable>();

        /// <summary>
        /// Weapon holstering event.
        /// When called will holster all currently active weapons.
        /// </summary>
        public ModelEvent holsterWeapon = new ModelEvent();

        #endregion

        #region Inventory
        /// <summary>
        /// Model function for retrieving ammo for a specific weapon.
        /// </summary>
        public ModelFunction<WeaponDefinition, int> getWeaponAmmo = new ModelFunction<WeaponDefinition, int>();

        /// <summary>
        /// Model function for setting ammo for a specific weapon.
        /// </summary>
        public ModelEvent<WeaponDefinition, int> setWeaponAmmo = new ModelEvent<WeaponDefinition, int>();

        /// <summary>
        /// All weapons currently in inventory.
        /// </summary>
        public ModelCollectionProperty<WeaponDefinition> availableWeapons = new ModelCollectionProperty<WeaponDefinition>();

        /// <summary>
        /// Attempts to pick up the specified weapon and/or the specified amount of ammunition for it.
        /// </summary>
        public ModelAttempt<WeaponAmmoTuple> pickupWeapon = new ModelAttempt<WeaponAmmoTuple>();

        /// <summary>
        /// Attempts to drop the specified weapon from the inventory.
        /// </summary>
        public ModelAttempt<WeaponDefinition> dropWeapon = new ModelAttempt<WeaponDefinition>();

        /// <summary>
        /// Called from <see cref="pickupWeapon"/> when a weapon actually was picked up.
        /// </summary>
        public ModelEvent<WeaponDefinition> onPickedupWeapon = new ModelEvent<WeaponDefinition>();

        /// <summary>
        /// Called from <see cref="dropWeapon"/> when a weapon actually was dropped.
        /// </summary>
        public ModelEvent<WeaponDefinition> onDroppedWeapon = new ModelEvent<WeaponDefinition>();

        #endregion
    }
}