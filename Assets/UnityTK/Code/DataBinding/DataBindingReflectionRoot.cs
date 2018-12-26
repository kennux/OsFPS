using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTK.DataBinding
{
    /// <summary>
	/// Abstract implementation for databinding roots which work by reflection.
	/// Used in <see cref="DataBindingRoot"/> and <see cref="DataBindingScriptedRoot"/>
    /// </summary>
    public abstract class DataBindingReflectionRoot : DataBindingReflectionNode
    {
        /// <summary>
        /// The update framerate.
        /// </summary>
        public int updateFramerate = 20;

        /// <summary>
        /// The update time calculated from <see cref="updateFramerate"/>
        /// </summary>
        private float updateTime { get { return 1f / (float)this.updateFramerate; } }

        /// <summary>
        /// Time passed since last update
        /// </summary>
        private float _time;

        /// <summary>
        /// Returns null always, since roots dont have a parent.
        /// </summary>
        public override DataBinding parent
        {
            get { return null; }
        }

        protected override void DoUpdateBinding()
        {

        }

        public void Update()
        {
            this._time += Time.deltaTime;

            if (this._time > this.updateTime)
            {
                this.UpdateBinding();
                this._time = 0;
            }
        }
    }
}