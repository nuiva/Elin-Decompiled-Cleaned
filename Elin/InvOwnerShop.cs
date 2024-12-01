public class InvOwnerShop : InvOwner
{
	public override bool HasTransaction => true;

	public override bool AllowTransfer => true;

	public InvOwnerShop(Card owner, Card container = null, CurrencyType _currency = CurrencyType.Money, PriceType _price = PriceType.Default)
		: base(owner, container, _currency, _price)
	{
	}

	public override void ListInteractions(ListInteraction list, Thing t, Trait trait, ButtonGrid b, bool context)
	{
	}

	public override bool OnCancelDrag(DragItemCard.DragInfo from)
	{
		return false;
	}
}
