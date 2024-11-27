using System;

public class InvOwnerUncurse : InvOwnerEffect
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
			return "invUncurse";
		}
	}

	public override string langWhat
	{
		get
		{
			return "target_what";
		}
	}

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.CreateScroll(this.superior ? 8241 : 8240, 1);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t.blessedState <= BlessedState.Cursed;
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(EffectId.Uncurse, 100, this.state, t.GetRootCard(), t, default(ActRef));
	}

	public InvOwnerUncurse() : base(null, null, CurrencyType.Money)
	{
	}
}
