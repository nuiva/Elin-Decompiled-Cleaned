using System;
using UnityEngine.EventSystems;

public class ButtonGridDrag : ButtonGrid, IDragHandler, IEventSystemHandler
{
	public IDragParent dragParent;

	[NonSerialized]
	public bool dragged;

	public override void SetDragParent(IDragParent p)
	{
		dragParent = p;
	}

	public void OnDrag(PointerEventData data)
	{
		if (dragParent != null && (!CanDragLeftButton || data.button == PointerEventData.InputButton.Left) && (CanDragLeftButton || data.button == PointerEventData.InputButton.Right))
		{
			if (!dragged)
			{
				SE.DragStart();
				dragged = true;
				dragParent.OnStartDrag(this);
				OnPointerUpOnDrag(data);
			}
			else
			{
				dragParent.OnDrag(this);
			}
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (dragged)
		{
			dragged = false;
			dragParent.OnEndDrag(this);
			UIInventory.RefreshAllList();
		}
		else
		{
			base.OnPointerUp(eventData);
		}
	}
}
