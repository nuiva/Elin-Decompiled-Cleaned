using UnityEngine;

public class DropdownRecipe : UIDropdown
{
	public static Color colorCost;

	public static Color colorPredict;

	public Recipe recipe;

	public UIText textLabel;

	public UIText orgLabel;

	public Recipe.Ingredient ingredient;

	public void RefreshLabel()
	{
		textLabel.SetText(GetLabel(ingredient.thing, recipe.source.colorIng == recipe.ingredients.IndexOf(ingredient)));
	}

	public string GetLabel(Thing t, bool showColor)
	{
		if (t == null)
		{
			return " <color=" + colorCost.ToHex() + ">" + "noMaterial".lang() + "</color>";
		}
		HitSummary summary = Core.Instance.screen.tileSelector.summary;
		int num = t.Num;
		return string.Concat((showColor ? ("<color=" + t.material.matColor.ToHex() + ">■</color> ") : "") + t.NameSimple + " (" + num + ")", (summary.countValid == 0) ? "" : (" <color=" + colorCost.ToHex() + "> -" + ingredient.req * summary.countValid + "</color>"));
	}
}
