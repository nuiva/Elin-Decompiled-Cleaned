using System;

public class TraitButcher : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "Butcher";
		}
	}

	public override bool CanUseFromInventory
	{
		get
		{
			return true;
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invButcher";
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
			return "cook_cut";
		}
	}
}
