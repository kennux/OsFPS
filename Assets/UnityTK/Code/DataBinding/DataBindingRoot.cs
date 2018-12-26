using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTK.DataBinding
{
    /// <summary>
    /// Implements a databinding root node that can be used to bind to an arbitrary unityengine object.
    /// </summary>
    public class DataBindingRoot : DataBindingReflectionRoot
    {
        /// <summary>
        /// The target object this root is binding to.
        /// </summary>
        [Header("Binding")]
        public UnityEngine.Object target;

        protected override Type GetBoundType()
        {
			var target = GetBoundObject();
            if (Essentials.UnityIsNull(target))
                return typeof(object);
            return target.GetType();
        }

        protected override object GetBoundObject()
        {
            return this.target;
        }
    }
}