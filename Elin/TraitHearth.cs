public class TraitHearth : TraitCooker
{
	public override ToggleType ToggleType => ToggleType.Fire;

	public override bool IsRestSpot => true;

	public override bool Contains(RecipeSource r)
	{
		return r.idFactory == "bonfire";
	}
}
