using System;

public class InvOwnerToolbelt : InvOwner
{
	public override bool AllowHold(Thing t)
	{
		return false;
	}

	public override void OnClick(ButtonGrid button)
	{
		SE.Beep();
	}

	public InvOwnerToolbelt(Card owner, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency, PriceType.Default)
	{
	}
}
