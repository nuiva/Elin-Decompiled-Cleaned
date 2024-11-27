using System;

public class TraitCauldron : TraitCookingPot
{
	public override string IDReqEle(RecipeSource r)
	{
		CardRow cardRow = r.row as CardRow;
		string result;
		if (!(((cardRow != null) ? cardRow._origin : null) == "dish"))
		{
			if ((result = base.GetParam(1, null)) == null)
			{
				return "handicraft";
			}
		}
		else
		{
			result = "cooking";
		}
		return result;
	}

	public override bool Contains(RecipeSource r)
	{
		return r.idFactory == "cauldron" || r.idFactory == "camppot";
	}
}
