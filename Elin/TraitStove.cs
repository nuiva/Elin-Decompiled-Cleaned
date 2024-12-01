public class TraitStove : TraitOven
{
	public override bool Contains(RecipeSource r)
	{
		if (!base.Contains(r))
		{
			return r.idFactory == "camppot";
		}
		return true;
	}
}
