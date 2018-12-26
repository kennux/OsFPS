using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK.DataBinding;

namespace UnityTK
{
	/// <summary>
	/// A view model implementation for <see cref="Tooltip"/>, which can show arbitrary strings as text.
	/// </summary>
	public class TextTooltipViewModel : TooltipViewModelBase<TextTooltipViewModel>
	{
		public string text { get; set; }

		public override DataBindingScriptedRoot prefab
		{
			get
			{
				if (Essentials.UnityIsNull(_prefab))
					_prefab = Resources.Load<GameObject>("UnityTK/UnityTKTextTooltip").GetComponent<DataBindingScriptedRoot>();
				return _prefab;
			}
		}
		private DataBindingScriptedRoot _prefab;
	}
}