using System;

public class TraitChopper : TraitCooker
{
	public override AnimeID IdAnimeProgress
	{
		get
		{
			return AnimeID.Shiver;
		}
	}

	public override string idSoundProgress
	{
		get
		{
			return "cook_cut";
		}
	}

	public override bool IdleUse(Chara c, int dist)
	{
		if (dist > 1)
		{
			return false;
		}
		this.owner.PlaySound("idle_cook", 1f, true);
		return true;
	}
}
