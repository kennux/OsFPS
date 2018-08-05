using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OsFPS
{
    /// <summary>
    /// Abstract and generic entity component implementation.
    /// This type assumes the entity its being attached to is always of type T.
    /// 
    /// It provides an additional getter for this type for convenience.
    /// It also outputs an error message if it was attached to the wrong entity type.
    /// </summary>
    /// <typeparam name="T">The expected entity type.</typeparam>
    public abstract class GenericEntityComponent<T> : EntityComponent where T : Entity
    {
        public T player
        {
            get
            {
                if (object.ReferenceEquals(this._playerEntity, null))
                    this._playerEntity = GetComponent<T>();
                return this._playerEntity;
            }
        }
        [NonSerialized]
        private T _playerEntity;

        public virtual void Awake()
        {
            this._playerEntity = GetComponent<T>();

            if (this._playerEntity == null)
            {
                Debug.Log("No entity of type " + typeof(T) + " on " + this.gameObject.name, this.gameObject);
            }
        }
    }
}