using System;

public class TraitCookerMicrowave : TraitCooker
{
	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Electronics;
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
			return "cook_micro";
		}
	}

	public override string idSoundComplete
	{
		get
		{
			return "cook_micro_finish";
		}
	}

	public override TraitCrafter.AnimeType animeType
	{
		get
		{
			return TraitCrafter.AnimeType.Microwave;
		}
	}

	public override bool AutoTurnOff
	{
		get
		{
			return true;
		}
	}

	public override bool AutoToggle
	{
		get
		{
			return false;
		}
	}
}
