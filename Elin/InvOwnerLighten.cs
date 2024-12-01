public class InvOwnerLighten : InvOwnerEffect
{
	public override bool CanTargetAlly => true;

	public override string langTransfer => "invLighten";

	public override string langWhat => "target_what";

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.CreateScroll(8280);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		if (t.trait is TraitTent)
		{
			return false;
		}
		if (t.trait.CanBeDropped && !t.category.IsChildOf("currency"))
		{
			if (state > BlessedState.Cursed)
			{
				return t.SelfWeight >= 100;
			}
			return true;
		}
		return false;
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(EffectId.Lighten, 100, state, t.GetRootCard(), t);
	}
}
