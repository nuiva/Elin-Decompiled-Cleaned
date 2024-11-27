using System;

public class TraitGrindstone : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "Grindstone";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invGrind";
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
}
