using System;

public class TraitHearth : TraitCooker
{
	public override ToggleType ToggleType
	{
		get
		{
			return ToggleType.Fire;
		}
	}

	public override bool IsRestSpot
	{
		get
		{
			return true;
		}
	}

	public override bool Contains(RecipeSource r)
	{
		return r.idFactory == "bonfire";
	}
}
