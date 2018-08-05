using UnityEngine;
using System.Collections;

namespace OsFPS
{
    /// <summary>
    /// Basic death handler for entities that immediately Destroys the entity gameobject.
    /// </summary>
    public class EntityInstantDespawnDeathHandler : EntityComponent
    {
        public override void OnRegisterEventHandlers()
        {
            this.entity.model.death.onStart += OnDie;
        }

        /// <summary>
        /// Immediately destroys the gameobject this component is attached to.
        /// Bound to <see cref="EntityModel.death"/>
        /// </summary>
        private void OnDie()
        {
            Destroy(this.gameObject);
        }
    }
}