using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK;
using UnityTK.BehaviourModel;

namespace OsFPS
{
    /// <summary>
    /// Base class for implementing components of <see cref="Entity"/>.
    /// Provides base functionality for implementing entity components (entity reference getter, entity event registering hook <see cref="OnRegisterEventHandlers"/>).
    /// </summary>
    [RequireComponent(typeof(Entity))]
    public abstract class EntityComponent : BehaviourModelComponent
    {
        /// <summary>
        /// The entity this component is linked to.
        /// </summary>
        public Entity entity
        {
            get
            {
                return this._entity.Get(this);
            }
        }
        private LazyLoadedComponentRef<Entity> _entity = new LazyLoadedComponentRef<Entity>();

        /// <summary>
        /// Can be overridden in order to bind to <see cref="EntityModel"/> from <see cref="Entity.model"/>.
        /// When events are being bound by this component, they should get bound in here!
        /// </summary>
        public virtual void OnRegisterEventHandlers()
        {

        }
    }
}