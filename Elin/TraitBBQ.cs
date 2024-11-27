using System;

public class TraitBBQ : TraitOven
{
	public override bool Contains(RecipeSource r)
	{
		return r.idFactory == "bonfire" || r.idFactory == "bbq";
	}
}
