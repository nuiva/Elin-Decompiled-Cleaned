using System;

public class TraitDyeMaker : TraitCrafter
{
	public override string IdSource
	{
		get
		{
			return "DyeMaker";
		}
	}

	public override string CrafterTitle
	{
		get
		{
			return "invBoil";
		}
	}

	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Fire;
		}
	}

	public override AnimeID IdAnimeProgress
	{
		get
		{
			return AnimeID.Shiver;
		}
	}

	public override TraitCrafter.AnimeType animeType
	{
		get
		{
			return TraitCrafter.AnimeType.Pot;
		}
	}

	public override string idSoundProgress
	{
		get
		{
			return "cook_pot";
		}
	}

	public override string idSoundBG
	{
		get
		{
			return "bg_boil";
		}
	}

	public override string IDReqEle(RecipeSource r)
	{
		return "alchemy";
	}

	public override int numIng
	{
		get
		{
			return 2;
		}
	}
}
