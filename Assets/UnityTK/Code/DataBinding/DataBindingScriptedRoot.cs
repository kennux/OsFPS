using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTK.DataBinding
{
    /// <summary>
    /// Implements a databinding root node that can be used to bind to an arbitrary objects.
	/// The bind target will not be settable via the inspector, but instead by directly setting <see cref="target"/>
    /// </summary>
    public class DataBindingScriptedRoot : DataBindingReflectionRoot
    {
		/// <summary>
		/// The bind target type.
		/// Used for <see cref="GetBoundType"/>
		/// </summary>
		public string bindTargetType;
		[ReadOnlyInspector]
		public bool bindTargetTypeValid;

        /// <summary>
        /// The target object this root is binding to.
		/// Must be of type <see cref="bindTargetType"/> or <see cref="ArgumentException"/> will be thrown.
        /// </summary>
        public object target
		{
			get { return _target; }
			set
			{
				if (!ReferenceEquals(value, null) && !ReferenceEquals(value.GetType(), GetBoundType()))
					throw new ArgumentException("Target must be of type " + this.bindTargetType);
				_target = value;
			}
		}
		private object _target;

		private Type bindTargetTypeCache;
		private string _bindTargetTypeCache;

        protected override Type GetBoundType()
        {
			if ((ReferenceEquals(bindTargetTypeCache, null) || !ReferenceEquals(bindTargetType, _bindTargetTypeCache)) && 
				!string.IsNullOrEmpty(this.bindTargetType))
			{
				bindTargetTypeCache = Type.GetType(bindTargetType);
				_bindTargetTypeCache = bindTargetType;
			}

			return bindTargetTypeCache;
        }

        protected override object GetBoundObject()
        {
            return this.target;
        }

		private void OnValidate()
		{
			this.bindTargetTypeValid = GetBoundType() != null;
		}
	}
}