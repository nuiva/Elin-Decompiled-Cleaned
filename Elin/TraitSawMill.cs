using System;

public class TraitSawMill : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "SawMill";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invSawMill";
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
			return "craft_saw";
		}
	}
}
