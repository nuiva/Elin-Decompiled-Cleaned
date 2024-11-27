using System;

public class InvOwnerBlend : InvOwnerDraglet
{
	public override bool CanTargetAlly
	{
		get
		{
			return true;
		}
	}

	public override string langTransfer
	{
		get
		{
			return "invBlend";
		}
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t != this.owner && this.owner.trait.CanBlend(t);
	}

	public InvOwnerBlend(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None) : base(owner, container, _currency)
	{
	}

	public override void _OnProcess(Thing t)
	{
		EClass.pc.Say("dip", EClass.pc, t, this.owner.GetName(NameStyle.Full, 1), null);
		SE.Change();
		this.owner.trait.OnBlend(t, t.GetRootCard().Chara ?? EClass.pc);
	}
}
