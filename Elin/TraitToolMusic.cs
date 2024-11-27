using System;

public class TraitToolMusic : TraitTool
{
	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.IsSelf)
		{
			this.TrySetAct(p);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct(new AI_PlayMusic
		{
			tool = this.owner.Thing
		}, this.owner);
	}
}
