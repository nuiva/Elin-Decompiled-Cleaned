public class TraitToolMusicBig : TraitToolMusic
{
	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct(new AI_PlayMusic
		{
			tool = owner.Thing
		}, owner);
	}
}
