public class TraitBBQ : TraitOven
{
	public override bool Contains(RecipeSource r)
	{
		if (!(r.idFactory == "bonfire"))
		{
			return r.idFactory == "bbq";
		}
		return true;
	}
}
