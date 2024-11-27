using System;

public class InvOwnerDeliverBox : InvOwnerDraglet
{
	public override string langTransfer
	{
		get
		{
			return "invDeliver";
		}
	}

	public InvOwnerDeliverBox(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t.category.IsChildOf("currency");
	}

	public override void _OnProcess(Thing t)
	{
		SE.Play("offering");
	}
}
