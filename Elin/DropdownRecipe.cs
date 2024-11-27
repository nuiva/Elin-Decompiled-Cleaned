using System;
using UnityEngine;

public class DropdownRecipe : UIDropdown
{
	public void RefreshLabel()
	{
		this.textLabel.SetText(this.GetLabel(this.ingredient.thing, this.recipe.source.colorIng == this.recipe.ingredients.IndexOf(this.ingredient)));
	}

	public string GetLabel(Thing t, bool showColor)
	{
		if (t == null)
		{
			return string.Concat(new string[]
			{
				" <color=",
				DropdownRecipe.colorCost.ToHex(),
				">",
				"noMaterial".lang(),
				"</color>"
			});
		}
		HitSummary summary = Core.Instance.screen.tileSelector.summary;
		int num = t.Num;
		return string.Concat(new string[]
		{
			showColor ? ("<color=" + t.material.matColor.ToHex() + ">■</color> ") : "",
			t.NameSimple,
			" (",
			num.ToString(),
			")"
		}) + ((summary.countValid == 0) ? "" : string.Concat(new string[]
		{
			" <color=",
			DropdownRecipe.colorCost.ToHex(),
			"> -",
			(this.ingredient.req * summary.countValid).ToString(),
			"</color>"
		}));
	}

	public static Color colorCost;

	public static Color colorPredict;

	public Recipe recipe;

	public UIText textLabel;

	public UIText orgLabel;

	public Recipe.Ingredient ingredient;
}
