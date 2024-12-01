public class TraitChopper : TraitCooker
{
	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "cook_cut";

	public override bool IdleUse(Chara c, int dist)
	{
		if (dist > 1)
		{
			return false;
		}
		owner.PlaySound("idle_cook");
		return true;
	}
}
