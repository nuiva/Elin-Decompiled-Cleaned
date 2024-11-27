using System;

public class InvOwnerAlly : InvOwner
{
	public override bool AllowTransfer
	{
		get
		{
			return false;
		}
	}

	public InvOwnerAlly(Card owner, Card container = null, CurrencyType _currency = CurrencyType.Money) : base(owner, container, _currency, PriceType.Default)
	{
	}

	public override void ListInteractions(InvOwner.ListInteraction list, Thing t, Trait trait, ButtonGrid b, bool context)
	{
	}

	public override bool OnCancelDrag(DragItemCard.DragInfo from)
	{
		return false;
	}

	public override void OnClick(ButtonGrid button)
	{
		SE.BeepSmall();
	}

	public override void OnRightClick(ButtonGrid button)
	{
		this.Process(button);
	}

	public override void OnRightPressed(ButtonGrid button)
	{
	}

	public bool Process(ButtonGrid button)
	{
		if (!button || button.card == null)
		{
			return false;
		}
		if (!LayerDragGrid.Instance)
		{
			return false;
		}
		ButtonGrid buttonGrid = LayerDragGrid.Instance.buttons[0];
		if (buttonGrid.card != null)
		{
			buttonGrid.card = null;
		}
		return new InvOwner.Transaction(new DragItemCard.DragInfo(button), new DragItemCard.DragInfo(buttonGrid), 1).Process(true);
	}

	public override string GetAutoUseLang(ButtonGrid button)
	{
		if (!button || button.card == null)
		{
			return null;
		}
		if (!new InvOwner.Transaction(new DragItemCard.DragInfo(button), new DragItemCard.DragInfo(LayerDragGrid.Instance.buttons[0]), 1).IsValid())
		{
			return null;
		}
		return LayerDragGrid.Instance.owner.langTransfer.lang();
	}
}
