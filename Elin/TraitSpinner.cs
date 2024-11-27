using System;

public class TraitSpinner : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "Spinner";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invSpinner";
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
			return "craft_spin";
		}
	}
}
