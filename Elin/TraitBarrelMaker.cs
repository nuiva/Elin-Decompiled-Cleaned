using System;

public class TraitBarrelMaker : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "BarrelMaker";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invMod";
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
			return "grind";
		}
	}

	public override string idSoundComplete
	{
		get
		{
			return "grind_finish";
		}
	}

	public override int numIng
	{
		get
		{
			return 2;
		}
	}

	public override bool StopSoundProgress
	{
		get
		{
			return true;
		}
	}

	public override bool IsConsumeIng
	{
		get
		{
			return false;
		}
	}

	public override bool ShouldConsumeIng(SourceRecipe.Row item, int index)
	{
		return false;
	}
}
