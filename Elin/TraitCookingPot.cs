using System;

public class TraitCookingPot : TraitCooker
{
	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Fire;
		}
	}

	public override AnimeID IdAnimeProgress
	{
		get
		{
			return AnimeID.Shiver;
		}
	}

	public override TraitCrafter.AnimeType animeType
	{
		get
		{
			return TraitCrafter.AnimeType.Pot;
		}
	}

	public override string idSoundProgress
	{
		get
		{
			return "cook_pot";
		}
	}

	public override string idSoundBG
	{
		get
		{
			return "bg_boil";
		}
	}
}
