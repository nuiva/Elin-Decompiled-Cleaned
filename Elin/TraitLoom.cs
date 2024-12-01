public class TraitLoom : TraitFactory
{
	public override bool Contains(RecipeSource r)
	{
		return r.idFactory == "loom";
	}
}
