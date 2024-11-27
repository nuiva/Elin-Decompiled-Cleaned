using System;

public class TraitStoneCutter : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "StoneCutter";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invStoneCutter";
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
