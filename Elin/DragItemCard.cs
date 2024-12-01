using System.Collections.Generic;

public class DragItemCard : DragItem
{
	public class DragInfo
	{
		public ButtonGrid button;

		public BodySlot equippedSlot;

		public int invX;

		public int invY;

		public Thing thing;

		public List<Thing> grid => container.things.grid;

		public InvOwner invOwner => button.invOwner;

		public Card container => invOwner.Container;

		public BaseList list => button.GetComponentInParent<BaseList>();

		public ContainerType ContainerType => container.trait.ContainerType;

		public DragInfo(ButtonGrid b)
		{
			button = b;
			if (!b || b.invOwner == null)
			{
				return;
			}
			if (b.card != null)
			{
				thing = b.card.Thing;
				invX = thing.invX;
				invY = thing.invY;
				if (thing.isEquipped && thing.GetRootCard() == invOwner.owner)
				{
					equippedSlot = invOwner.owner.Chara.body.slots[thing.c_equippedSlot - 1];
				}
			}
			else
			{
				invX = b.index;
				invY = b.invOwner.destInvY;
			}
		}
	}

	public ButtonGrid button;

	public DragInfo from;

	public DragInfo to;

	public override UIButton Button => button;

	public DragItemCard(ButtonGrid _button, bool setDragImage = true)
	{
		button = _button;
		from = new DragInfo(button);
		if (setDragImage)
		{
			EClass.ui.hud.SetDragImage(button.icon, null, button.mainText);
		}
	}

	public override void OnStartDrag()
	{
		from.invOwner.OnStartDrag(from);
		if ((bool)LayerDragGrid.Instance && LayerDragGrid.Instance.owner.ShouldShowGuide(from.thing))
		{
			LayerDragGrid.Instance.CurrentButton.Attach("guide", rightAttach: false);
		}
	}

	public override void OnEndDrag()
	{
		WidgetEquip.dragEquip = null;
		WidgetEquip.Redraw();
		if ((bool)LayerDragGrid.Instance)
		{
			LayerDragGrid.Instance.CurrentButton.Dettach("guide");
		}
	}

	public override bool OnDragSpecial()
	{
		DragInfo dragInfo = new DragInfo(InputModuleEX.GetComponentOf<ButtonGrid>());
		if (dragInfo != null && (bool)dragInfo.button && dragInfo.button.interactable && !(dragInfo.invOwner is InvOwnerHotbar) && dragInfo.invOwner.owner == from.invOwner.owner && dragInfo.invOwner.owner.IsPC && from.thing.Num > 1 && (dragInfo.thing == null || dragInfo.thing.CanStackTo(from.thing)))
		{
			bool num = dragInfo.thing != null;
			Thing thing = from.thing.Split(1);
			SE.Play(thing.material.GetSoundDrop(thing.sourceCard));
			if (num)
			{
				dragInfo.thing.ModNum(1);
			}
			else
			{
				dragInfo.invOwner.Container.AddThing(thing, tryStack: false);
				thing.invX = dragInfo.invX;
				thing.invY = dragInfo.invY;
			}
			return true;
		}
		return false;
	}

	public override bool OnDrag(bool execute, bool cancel = false)
	{
		DragInfo dragInfo = new DragInfo(InputModuleEX.GetComponentOf<ButtonGrid>());
		bool num = from.invOwner.OnDrag(from, dragInfo, execute, cancel);
		if (num && EClass.core.config.game.doubleClickToHold && EClass.ui.dragDuration < 0.35f && dragInfo.button == from.button && from.thing != null)
		{
			dragInfo.button.invOwner.TryHold(from.thing);
		}
		return num;
	}
}
