public class InvOwnerUncurse : InvOwnerEffect
{
	public override bool CanTargetAlly => true;

	public override string langTransfer => "invUncurse";

	public override string langWhat => "target_what";

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.CreateScroll(superior ? 8241 : 8240);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t.blessedState <= BlessedState.Cursed;
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(EffectId.Uncurse, 100, state, t.GetRootCard(), t);
	}
}
