using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK.DataBinding;

namespace UnityTK
{
	/// <summary>
	/// A UGUI tooltip implementation.
	/// This tooltip implementation can present arbitrary content to the user and is used together with UnityTK databindings.
	/// When opening the tooltip, it will be opened with a data model that defines it's data, this data model will be passed to the <see cref="DataBindingScriptedRoot"/> on the viewmodel's prefab.
	/// <see cref="TooltipViewModel.prefab"/>
	/// 
	/// Other components of UnityTK's tooltip system:
	/// <see cref="TooltipViewModel"/>
	/// <see cref="TooltipAnchorTarget"/>
	/// 
	/// Example implementation of the view model:
	/// <see cref="TextTooltipViewModel"/>
	/// </summary>
	[RequireComponent(typeof(CanvasGroup))]
	public class Tooltip : SingletonBehaviour<Tooltip>
	{
		/// <summary>
		/// The transform used as parent for content objects.
		/// </summary>
		public Transform contentAnchor;

		/// <summary>
		/// Prefab instance cache for tooltip content, <see cref="TooltipViewModel.prefab"/>
		/// </summary>
		private Dictionary<DataBindingScriptedRoot, DataBindingScriptedRoot> contentPrefabInstances = new Dictionary<DataBindingScriptedRoot, DataBindingScriptedRoot>();
		
		/// <summary>
		/// The canvas group of this tooltip, used to show / hide it (via <see cref="CanvasGroup.alpha"/>).
		/// Lazy-loaded component reference.
		/// </summary>
		private CanvasGroup canvasGroup
		{
			get { return _canvasGroup.Get(this); }
		}
		private LazyLoadedComponentRef<CanvasGroup> _canvasGroup;
		
		/// <summary>
		/// The canvas this tooltip is existing in, this is fetched via GetComponentInParent.
		/// Lazy-loaded component reference.
		/// </summary>
		private Canvas canvas
		{
			get
			{
				if (Essentials.UnityIsNull(_canvas))
					_canvas = GetComponentInParent<Canvas>();
				return _canvas;
			}
		}
		private Canvas _canvas;
		
		/// <summary>
		/// The rect transform of the tooltip.
		/// Lazy-loaded component reference.
		/// </summary>
		private RectTransform rectTransform
		{
			get { return _rectTransform.Get(this); }
		}
		private LazyLoadedComponentRef<RectTransform> _rectTransform;

		/// <summary>
		/// The currently enabled content.
		/// If no content is enabled, this is set to null.
		/// </summary>
		private DataBindingScriptedRoot currentContent;

		/// <summary>
		/// The anchor currently used to position the tooltip.
		/// </summary>
		private TooltipAnchorTarget anchor;

		/// <summary>
		/// Opens the tooltip with the specified view model and anchor.
		/// If there is no tooltip instance it will output a warning, unless unity is running in headless mode.
		/// </summary>
		public static void Open<T>(T model, TooltipAnchorTarget anchor = default(TooltipAnchorTarget)) where T : TooltipViewModel
		{
			if (ReferenceEquals(instance, null))
			{
				if (!Essentials.IsRunningHeadless()) // Servers and other headless apps dont care about tooltips
					Debug.LogWarning("Tried opening tooltip with no tooltip in the scene!");
				return;
			}
			
			instance._Open(model, anchor);
		}

		/// <summary>
		/// Immediately closes the tooltip.
		/// If there is no tooltip instance it will output a warning, unless unity is running in headless mode.
		/// </summary>
		public static void Close()
		{
			if (ReferenceEquals(instance, null))
			{
				if (!Essentials.IsRunningHeadless()) // Servers and other headless apps dont care about tooltips
					Debug.LogWarning("Tried closing tooltip with no tooltip in the scene!");
				return;
			}

			instance._Close();
		}

		/// <summary>
		/// <see cref="Close"/>
		/// </summary>
		private void _Close()
		{
			if (!ReferenceEquals(this.currentContent, null))
			{
				this.currentContent.gameObject.SetActive(false);
				this.currentContent = null;
			}
		}

		/// <summary>
		/// <see cref="Open{T}(T, TooltipAnchorTarget)"/>
		/// </summary>
		private void _Open<T>(T model, TooltipAnchorTarget anchor) where T : TooltipViewModel
		{
			_Close();
			if (!ReferenceEquals(this.currentContent, null))
				this.currentContent.gameObject.SetActive(false);

			currentContent = GetContentPrefabInstance(model.prefab);
			currentContent.gameObject.SetActive(true);
			currentContent.target = model;
			currentContent.UpdateBinding();
			this.anchor = anchor;
			UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
		}

		/// <summary>
		/// Gets a content prefab instance.
		/// This instance is either already existing (<see cref="contentPrefabInstances"/>) or created and added to the instance dictionary.
		/// </summary>
		private DataBindingScriptedRoot GetContentPrefabInstance(DataBindingScriptedRoot prefab)
		{
			DataBindingScriptedRoot instance;
			if (!this.contentPrefabInstances.TryGetValue(prefab, out instance))
			{
				// Validate
				var go = Instantiate(prefab.gameObject);
				go.transform.parent = this.contentAnchor;
				go.SetActive(false);

				instance = go.GetComponent<DataBindingScriptedRoot>();
				this.contentPrefabInstances.Add(prefab, instance);
			}

			return instance;
		}

		/// <summary>
		/// Updates tooltip position and will close tooltip if the anchor was destroyed.
		/// </summary>
		private void Update()
		{
			bool isActive = !ReferenceEquals(this.currentContent, null);
			this.canvasGroup.alpha = isActive ? 1 : 0;

			if (isActive && !this.anchor.UpdateTooltipPosition(this.rectTransform, canvas))
				Close();
		}
	}
}