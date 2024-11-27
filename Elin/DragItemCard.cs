using System;
using System.Collections.Generic;

public class DragItemCard : DragItem
{
	public DragItemCard(ButtonGrid _button, bool setDragImage = true)
	{
		this.button = _button;
		this.from = new DragItemCard.DragInfo(this.button);
		if (setDragImage)
		{
			EClass.ui.hud.SetDragImage(this.button.icon, null, this.button.mainText);
		}
	}

	public override UIButton Button
	{
		get
		{
			return this.button;
		}
	}

	public override void OnStartDrag()
	{
		this.from.invOwner.OnStartDrag(this.from);
		if (LayerDragGrid.Instance && LayerDragGrid.Instance.owner.ShouldShowGuide(this.from.thing))
		{
			LayerDragGrid.Instance.CurrentButton.Attach("guide", false);
		}
	}

	public override void OnEndDrag()
	{
		WidgetEquip.dragEquip = null;
		WidgetEquip.Redraw();
		if (LayerDragGrid.Instance)
		{
			LayerDragGrid.Instance.CurrentButton.Dettach("guide");
		}
	}

	public override bool OnDragSpecial()
	{
		DragItemCard.DragInfo dragInfo = new DragItemCard.DragInfo(InputModuleEX.GetComponentOf<ButtonGrid>());
		if (dragInfo != null && dragInfo.button && dragInfo.button.interactable && !(dragInfo.invOwner is InvOwnerHotbar) && dragInfo.invOwner.owner == this.from.invOwner.owner && dragInfo.invOwner.owner.IsPC && this.from.thing.Num > 1 && (dragInfo.thing == null || dragInfo.thing.CanStackTo(this.from.thing)))
		{
			bool flag = dragInfo.thing != null;
			Thing thing = this.from.thing.Split(1);
			SE.Play(thing.material.GetSoundDrop(thing.sourceCard));
			if (flag)
			{
				dragInfo.thing.ModNum(1, true);
			}
			else
			{
				dragInfo.invOwner.Container.AddThing(thing, false, -1, -1);
				thing.invX = dragInfo.invX;
				thing.invY = dragInfo.invY;
			}
			return true;
		}
		return false;
	}

	public override bool OnDrag(bool execute, bool cancel = false)
	{
		DragItemCard.DragInfo dragInfo = new DragItemCard.DragInfo(InputModuleEX.GetComponentOf<ButtonGrid>());
		bool flag = this.from.invOwner.OnDrag(this.from, dragInfo, execute, cancel);
		if (flag && EClass.core.config.game.doubleClickToHold && EClass.ui.dragDuration < 0.35f && dragInfo.button == this.from.button && this.from.thing != null)
		{
			dragInfo.button.invOwner.TryHold(this.from.thing);
		}
		return flag;
	}

	public ButtonGrid button;

	public DragItemCard.DragInfo from;

	public DragItemCard.DragInfo to;

	public class DragInfo
	{
		public DragInfo(ButtonGrid b)
		{
			this.button = b;
			if (b && b.invOwner != null)
			{
				if (b.card != null)
				{
					this.thing = b.card.Thing;
					this.invX = this.thing.invX;
					this.invY = this.thing.invY;
					if (this.thing.isEquipped && this.thing.GetRootCard() == this.invOwner.owner)
					{
						this.equippedSlot = this.invOwner.owner.Chara.body.slots[this.thing.c_equippedSlot - 1];
						return;
					}
				}
				else
				{
					this.invX = b.index;
					this.invY = b.invOwner.destInvY;
				}
			}
		}

		public List<Thing> grid
		{
			get
			{
				return this.container.things.grid;
			}
		}

		public InvOwner invOwner
		{
			get
			{
				return this.button.invOwner;
			}
		}

		public Card container
		{
			get
			{
				return this.invOwner.Container;
			}
		}

		public BaseList list
		{
			get
			{
				return this.button.GetComponentInParent<BaseList>();
			}
		}

		public ContainerType ContainerType
		{
			get
			{
				return this.container.trait.ContainerType;
			}
		}

		public ButtonGrid button;

		public BodySlot equippedSlot;

		public int invX;

		public int invY;

		public Thing thing;
	}
}
