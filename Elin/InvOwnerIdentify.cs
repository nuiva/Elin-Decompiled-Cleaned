using System;

public class InvOwnerIdentify : InvOwnerEffect
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
			return "invIdentify";
		}
	}

	public override string langWhat
	{
		get
		{
			return "identify_what";
		}
	}

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.CreateScroll(this.superior ? 8232 : 8230, 1);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return !t.IsIdentified;
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(this.superior ? EffectId.GreaterIdentify : EffectId.Identify, 100, this.state, t.GetRootCard(), t, default(ActRef));
		if (t.GetRootCard().IsPC)
		{
			EClass.core.actionsNextFrame.Add(delegate
			{
				EClass.core.actionsNextFrame.Add(delegate
				{
					UIButton.TryShowTip(null, true, false);
				});
			});
		}
	}

	public InvOwnerIdentify() : base(null, null, CurrencyType.Money)
	{
	}
}
