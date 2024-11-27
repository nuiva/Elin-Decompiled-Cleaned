using System;

public class InvOwnerOffering : InvOwnerDraglet
{
	public override string langTransfer
	{
		get
		{
			return "invOffering";
		}
	}

	public override InvOwnerDraglet.ProcessType processType
	{
		get
		{
			return InvOwnerDraglet.ProcessType.Consume;
		}
	}

	public override bool DenyImportant
	{
		get
		{
			return true;
		}
	}

	public InvOwnerOffering(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money) : base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return this.altar.CanOffer(t);
	}

	public override void _OnProcess(Thing t)
	{
		this.altar.OnOffer(EClass.pc, t);
	}

	public TraitAltar altar;
}
