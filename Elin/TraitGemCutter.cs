using System;

public class TraitGemCutter : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "GemCutter";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invGemCutter";
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

	public override int MaxFuel
	{
		get
		{
			return 100;
		}
	}

	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Electronics;
		}
	}
}
