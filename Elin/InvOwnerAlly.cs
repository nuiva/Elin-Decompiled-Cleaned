public class InvOwnerAlly : InvOwner
{
	public override bool AllowTransfer => false;

	public InvOwnerAlly(Card owner, Card container = null, CurrencyType _currency = CurrencyType.Money)
		: base(owner, container, _currency)
	{
	}

	public override void ListInteractions(ListInteraction list, Thing t, Trait trait, ButtonGrid b, bool context)
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
		Process(button);
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
		return new Transaction(new DragItemCard.DragInfo(button), new DragItemCard.DragInfo(buttonGrid)).Process(startTransaction: true);
	}

	public override string GetAutoUseLang(ButtonGrid button)
	{
		if (!button || button.card == null)
		{
			return null;
		}
		if (!new Transaction(new DragItemCard.DragInfo(button), new DragItemCard.DragInfo(LayerDragGrid.Instance.buttons[0])).IsValid())
		{
			return null;
		}
		return LayerDragGrid.Instance.owner.langTransfer.lang();
	}
}
