using System;

public class TraitToolMusicBig : TraitToolMusic
{
	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct(new AI_PlayMusic
		{
			tool = this.owner.Thing
		}, this.owner);
	}
}
