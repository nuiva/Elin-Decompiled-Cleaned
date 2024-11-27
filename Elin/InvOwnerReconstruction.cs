using System;

public class InvOwnerReconstruction : InvOwnerEffect
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
			return "invReconstruct";
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
		return ThingGen.CreateScroll(8288, 1);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return !t.HasTag(CTAG.godArtifact) && !(t.trait is TraitTent) && !(t.trait is TraitStairs) && t.trait.CanBeDropped && !t.category.IsChildOf("currency") && t.IsEquipment && !t.isReplica;
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(EffectId.Reconstruction, 100, this.state, t.GetRootCard(), t, default(ActRef));
	}

	public InvOwnerReconstruction() : base(null, null, CurrencyType.Money)
	{
	}
}
