using System;

public class InvOwnerGive : InvOwnerDraglet
{
	public override string langTransfer
	{
		get
		{
			return "invGive";
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

	public InvOwnerGive(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money) : base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return !t.trait.CanOnlyCarry;
	}

	public override void _OnProcess(Thing t)
	{
		SE.Play("offering");
	}

	public Chara chara;
}
