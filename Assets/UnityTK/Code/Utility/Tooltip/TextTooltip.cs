using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityTK
{
	/// <summary>
	/// Tooltip opener implementation for <see cref="TextTooltipViewModel"/>.
	/// Will open a tooltip with the specified text on the gameobject this is attached to or the mouse position.
	/// </summary>
	public class TextTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public string text;
		public bool anchorToMouse = false;
		public bool followMouse = false;

		[Header("Only for RectTransforms")]
		public Vector2 tooltipPivotInRectTransform = new Vector2(.5f,.5f);

		public void OnPointerEnter(PointerEventData eventData)
		{
			var model = TextTooltipViewModel.instance;
			model.text = text;

			TooltipAnchorTarget target;
			if (this.anchorToMouse)
				target = TooltipAnchorTarget.ForMouse(this.followMouse);
			else if (this.transform is RectTransform)
				target = TooltipAnchorTarget.ForUIObject(this.transform as RectTransform, this.tooltipPivotInRectTransform);
			else
				target = TooltipAnchorTarget.ForWorldObject(this.transform);

			Tooltip.Open(model, target);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Tooltip.Close();
		}
	}
}
