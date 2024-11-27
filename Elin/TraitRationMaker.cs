using System;

public class TraitRationMaker : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "RationMaker";
		}
	}

	public override int numIng
	{
		get
		{
			return 2;
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invMill";
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
			return "cook_grind";
		}
	}
}
