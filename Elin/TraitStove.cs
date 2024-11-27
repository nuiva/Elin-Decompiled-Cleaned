using System;

public class TraitStove : TraitOven
{
	public override bool Contains(RecipeSource r)
	{
		return base.Contains(r) || r.idFactory == "camppot";
	}
}
