using System;

public class InvOwnerLighten : InvOwnerEffect
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
			return "invLighten";
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
		return ThingGen.CreateScroll(8280, 1);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return !(t.trait is TraitTent) && (t.trait.CanBeDropped && !t.category.IsChildOf("currency")) && (this.state <= BlessedState.Cursed || t.SelfWeight >= 100);
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(EffectId.Lighten, 100, this.state, t.GetRootCard(), t, default(ActRef));
	}

	public InvOwnerLighten() : base(null, null, CurrencyType.Money)
	{
	}
}
