using System;

public class TraitScratchMachine : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "Scratch";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invScratch";
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
			return "craft_scratch";
		}
	}
}
