using System;

public class TraitSculpture : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "Sculpture";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invSculpt";
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
			return "craft_sculpt";
		}
	}
}
