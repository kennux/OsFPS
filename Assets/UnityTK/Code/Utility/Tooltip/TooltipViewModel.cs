using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK.DataBinding;

namespace UnityTK
{
	/// <summary>
	/// Abstract class to implement viewmodels for <see cref="Tooltip"/>.
	/// <see cref="TooltipViewModelBase{T}"/>
	/// </summary>
	public abstract class TooltipViewModel
	{
		/// <summary>
		/// The prefab to be used for creating the content, parented to the tooltip content anchor.
		/// This will only be instantiated once per TooltipViewModel implementation type and constantly reused.
		/// </summary>
		public abstract DataBindingScriptedRoot prefab { get; }
	}

	/// <summary>
	/// Base class for implementing tooltip view models with a singleton pattern.
	/// This should always be used as base class for tooltip view models.
	/// 
	/// The singleton pattern causes reuse of one instance across the entire application lifetime without any memory allocations at runtime.
	/// </summary>
	public abstract class TooltipViewModelBase<T> : TooltipViewModel where T : TooltipViewModelBase<T>, new()
	{
		public static T instance = new T();
	}
}