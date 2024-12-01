public class InvOwnerIdentify : InvOwnerEffect
{
	public override bool CanTargetAlly => true;

	public override string langTransfer => "invIdentify";

	public override string langWhat => "identify_what";

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.CreateScroll(superior ? 8232 : 8230);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return !t.IsIdentified;
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(superior ? EffectId.GreaterIdentify : EffectId.Identify, 100, state, t.GetRootCard(), t);
		if (!t.GetRootCard().IsPC)
		{
			return;
		}
		EClass.core.actionsNextFrame.Add(delegate
		{
			EClass.core.actionsNextFrame.Add(delegate
			{
				UIButton.TryShowTip(null, highlight: true, ignoreWhenRightClick: false);
			});
		});
	}
}
