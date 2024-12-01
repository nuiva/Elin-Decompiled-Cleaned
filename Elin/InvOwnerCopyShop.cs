public class InvOwnerCopyShop : InvOwner
{
	public override bool UseGuide => true;

	public InvOwnerCopyShop(Card owner, Card container = null, CurrencyType _currency = CurrencyType.None)
		: base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return owner.trait.CanCopy(t);
	}

	public override bool AllowMoved(Thing t)
	{
		return owner.trait.CanCopy(t);
	}
}
