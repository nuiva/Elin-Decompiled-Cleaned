public class TraitCauldron : TraitCookingPot
{
	public override string IDReqEle(RecipeSource r)
	{
		object obj;
		if (!((r.row as CardRow)?._origin == "dish"))
		{
			obj = GetParam(1);
			if (obj == null)
			{
				return "handicraft";
			}
		}
		else
		{
			obj = "cooking";
		}
		return (string)obj;
	}

	public override bool Contains(RecipeSource r)
	{
		if (!(r.idFactory == "cauldron"))
		{
			return r.idFactory == "camppot";
		}
		return true;
	}
}
