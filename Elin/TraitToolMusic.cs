public class TraitToolMusic : TraitTool
{
	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.IsSelf)
		{
			TrySetAct(p);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct(new AI_PlayMusic
		{
			tool = owner.Thing
		}, owner);
	}
}
