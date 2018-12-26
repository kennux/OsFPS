using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTK.DataBinding;

namespace UnityTK
{
	/// <summary>
	/// Tooltip anchoring struct to be used to define how the tooltip will position upon being opened.
	/// 
	/// There are 3 anchoring modes:
	/// - World object anchor
	/// -> Tooltip will anchor to a world object directly, the tooltip pivot will be in the same position as the world object on the screen.
	/// - UI object anchor
	/// -> Tooltip will anchor to a ui object (recttransform)
	/// - Mouse anchor
	/// -> Tooltip will anchor to the mouse position (can be locked to the mouse position at the time of opening, if not locked it will follow the mouse)
	/// </summary>
	public struct TooltipAnchorTarget
	{
		private Transform worldAnchor;
		private RectTransform uiAnchor;

		/// <summary>
		/// The local coordinates in the rectangle of <see cref="uiAnchor"/> that will be translated to canvas coordinates.
		/// The coordinates are in 0-1 range, normalized but can be overtuned (less than 0 or greater than 1).
		/// 
		/// 0|0 is bottom left corner
		/// 1|1 is top right corner
		/// </summary>
		private Vector2 uiAnchorPivot;
		private Vector3? fixedMousePos;

		/// <summary>
		/// World object anchoring <see cref="TooltipAnchorTarget"/> summary.
		/// </summary>
		public static TooltipAnchorTarget ForWorldObject(Transform transform)
		{
			return new TooltipAnchorTarget()
			{
				worldAnchor = transform
			};
		}
		
		/// <summary>
		/// UI object anchoring <see cref="TooltipAnchorTarget"/> summary.
		/// </summary>
		/// <param name="uiAnchorPivot"><see cref="uiAnchorPivot"/></param>
		public static TooltipAnchorTarget ForUIObject(RectTransform transform, Vector2 uiAnchorPivot)
		{
			return new TooltipAnchorTarget()
			{
				uiAnchor = transform,
				uiAnchorPivot = uiAnchorPivot
			};
		}

		/// <summary>
		/// Mouse anchoring <see cref="TooltipAnchorTarget"/> summary.
		/// </summary>
		/// <param name="followMouse">Whether or not the tooltip will follow the mouse over its lifetime.</param>
		public static TooltipAnchorTarget ForMouse(bool followMouse = false)
		{
			if (followMouse)
				return new TooltipAnchorTarget();
			else
				return new TooltipAnchorTarget()
				{
					fixedMousePos = Input.mousePosition
				};
		}

		/// <summary>
		/// Updates the tooltip position, called every frame from <see cref="Tooltip.Update"/>.
		/// </summary>
		/// <param name="tooltip">The tooltip transform.</param>
		/// <param name="tooltipCanvas">The parent canvas.</param>
		/// <returns>Whether or not the tooltip could be positioned, this is false if the anchors have been destroyed.</returns>
		public bool UpdateTooltipPosition(RectTransform tooltip, Canvas tooltipCanvas)
		{
			if (!ReferenceEquals(this.worldAnchor, null))
			{
				if (Essentials.UnityIsNull(this.worldAnchor))
					return false;

				tooltip.position = Camera.main.WorldToScreenPoint(this.worldAnchor.position);
			}
			else if (!ReferenceEquals(this.uiAnchor, null))
			{
				if (Essentials.UnityIsNull(this.uiAnchor))
					return false;
				
				float x = this.uiAnchor.position.x + Mathf.Lerp(this.uiAnchor.rect.xMin, this.uiAnchor.rect.xMax, this.uiAnchorPivot.x);
				float y = this.uiAnchor.position.y + Mathf.Lerp(this.uiAnchor.rect.yMin, this.uiAnchor.rect.yMax, this.uiAnchorPivot.y);
				tooltip.position = new Vector2(x, y);
			}
			else
			{
				Vector2 pos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(tooltipCanvas.transform as RectTransform, this.fixedMousePos.HasValue ? this.fixedMousePos.Value : Input.mousePosition, tooltipCanvas.worldCamera, out pos);
				tooltip.position = tooltipCanvas.transform.TransformPoint(pos);
			}

			return true;
		}
	}
}
