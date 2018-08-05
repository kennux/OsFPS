using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Basic damage handler implementation that has a finite amount of healthpoints and just takes damage on being hit.
    /// If the entity dies <see cref="EntityModel.death"/> is fired.
    /// </summary>
    public class EntityDamageHandler : EntityComponent, IDamageHandler
    {
        /// <summary>
        /// The current health
        /// </summary>
        [Header("State")]
        public float health;

        /// <summary>
        /// The maximum health
        /// </summary>
        public float maxHealth { get { return this.entity.model.maxHealth.Get(); } }

        public override void OnRegisterEventHandlers()
        {
            this.entity.model.health.SetGetter(() => this.health);
            this.entity.model.health.onSetValue += this.SetHealth;
            this.entity.model.onDamageTaken.handler += OnHandleDamage;

            this.entity.model.death.RegisterStartCondition(CanDie);
            this.entity.model.death.RegisterActivityGetter(IsDead);
        }

        /// <summary>
        /// Setter of <see cref="EntityModel.health"/>
        /// </summary>
        protected virtual void SetHealth(float health)
        {
            this.health = health;
            if (this.health <= 0)
                this.entity.model.death.ForceStart(); // CanDie will always be met here
        }

        /// <summary>
        /// Condition for <see cref="EntityModel.death"/>
        /// </summary>
        protected virtual bool CanDie()
        {
            return CanDie();
        }

        /// <summary>
        /// Activity-getter for <see cref="EntityModel.death"/>
        /// </summary>
        protected virtual bool IsDead()
        {
            return this.health <= 0;
        }


        /// <summary>
        /// Handler for <see cref="EntityModel.onDamageTaken"/>
        /// </summary>
        protected virtual void OnHandleDamage(DamageEventArgs args)
        {
            this.health -= args.damage * this.entity.model.damageTaken.Get();
            if (this.health <= 0)
                this.entity.model.death.ForceStart(); // CanDie will always be met here
        }

        /// <summary>
        /// Fires <see cref="EntityModel.onDamageTaken"/>, implemented from <see cref="IDamageHandler.TakeDamage(DamageEventArgs)"/>
        /// </summary>
        public void TakeDamage(DamageEventArgs args)
        {
            this.entity.model.onDamageTaken.Fire(args);
        }
    }
}