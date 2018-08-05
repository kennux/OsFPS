using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// The base class for every entity.
    /// It serves as main class of entities, which are composed of an entity object and a collection of <see cref="EntityComponent"/> components.
    /// 
    /// There is a set of components all entites should have:
    /// - Damage handler
    /// - Weapon handler
    /// - Controller
    /// </summary>
    public class Entity : MonoBehaviour
    {
#if UNITY_EDITOR
        public void OnValidate()
        {
            if (this.controller == null)
                this.controller = GetComponent<EntityController>();
            if (this.model == null)
                this.model = GetComponent<EntityModel>();
        }
#endif

        /// <summary>
        /// The controller of this entity.
        /// </summary>
        [Header("Required components")]
        public EntityController controller;

        /// <summary>
        /// The model of the entity.
        /// </summary>
        public EntityModel model;

        /// <summary>
        /// All components of this entity.
        /// </summary>
        [Header("Runtime debug")]
        [SerializeField]
        protected List<EntityComponent> components;

        public void Start()
        {
            this.components = new List<EntityComponent>(GetComponents<EntityComponent>());

            foreach (var c in this.components)
                c.OnRegisterEventHandlers();
        }
    }
}