using System;

public class TraitKiln : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "Kiln";
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
			return "invKiln";
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
