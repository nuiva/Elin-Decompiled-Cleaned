using System;

public class TraitSmelter : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "Smelter";
		}
	}

	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Fire;
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invSmelter";
		}
	}

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
			return "craft_smelt";
		}
	}
}
