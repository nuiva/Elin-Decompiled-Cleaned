using System;
using UnityEngine.EventSystems;

public class ButtonGridDrag : ButtonGrid, IDragHandler, IEventSystemHandler
{
	public override void SetDragParent(IDragParent p)
	{
		this.dragParent = p;
	}

	public void OnDrag(PointerEventData data)
	{
		if (this.dragParent == null)
		{
			return;
		}
		if (this.CanDragLeftButton && data.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		if (!this.CanDragLeftButton && data.button != PointerEventData.InputButton.Right)
		{
			return;
		}
		if (!this.dragged)
		{
			SE.DragStart();
			this.dragged = true;
			this.dragParent.OnStartDrag(this);
			base.OnPointerUpOnDrag(data);
			return;
		}
		this.dragParent.OnDrag(this);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (this.dragged)
		{
			this.dragged = false;
			this.dragParent.OnEndDrag(this, false);
			UIInventory.RefreshAllList();
			return;
		}
		base.OnPointerUp(eventData);
	}

	public IDragParent dragParent;

	[NonSerialized]
	public bool dragged;
}
