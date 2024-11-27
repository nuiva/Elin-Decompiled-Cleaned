using System;

public class TraitIncubator : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "Incubator";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invIncubator";
		}
	}

	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Electronics;
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
			return "egg";
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

	public override bool CanUseFromInventory
	{
		get
		{
			return false;
		}
	}
}
