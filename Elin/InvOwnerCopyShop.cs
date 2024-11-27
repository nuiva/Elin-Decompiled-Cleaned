using System;

public class InvOwnerCopyShop : InvOwner
{
	public InvOwnerCopyShop(Card owner, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency, PriceType.Default)
	{
	}

	public override bool UseGuide
	{
		get
		{
			return true;
		}
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return this.owner.trait.CanCopy(t);
	}

	public override bool AllowMoved(Thing t)
	{
		return this.owner.trait.CanCopy(t);
	}
}
