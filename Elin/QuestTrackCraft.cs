using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class QuestTrackCraft : Quest
{
	public RecipeSource recipe
	{
		get
		{
			RecipeSource result;
			if ((result = this._recipe) == null)
			{
				result = (this._recipe = RecipeManager.Get(this.idRecipe));
			}
			return result;
		}
	}

	public override string GetTitle()
	{
		return "trackCraft_title".lang(this.recipe.Name, null, null, null, null);
	}

	public void SetRecipe(Recipe r)
	{
		this.idRecipe = r.id;
	}

	public override string GetDetail(bool onJournal = false)
	{
		if (this.ingredients == null)
		{
			this.ingredients = this.recipe.GetIngredients();
		}
		string text = "";
		for (int i = 0; i < this.ingredients.Count; i++)
		{
			Recipe.Ingredient ingredient = this.ingredients[i];
			if (!text.IsEmpty())
			{
				text += Environment.NewLine;
			}
			ThingStack thingStack = EClass._map.Stocked.ListThingStack(ingredient, StockSearchMode.All);
			List<Thing> list = thingStack.list;
			Thing thing = null;
			foreach (Thing thing2 in thingStack.list)
			{
				if (thing == null || thing2.Num > thing.Num)
				{
					thing = thing2;
				}
			}
			int num = (thing == null) ? 0 : thing.Num;
			if (num >= ingredient.req)
			{
				text += "trackCraft_met".lang().TagColor(FontColor.Good, null);
			}
			text = string.Concat(new string[]
			{
				text,
				ingredient.optional ? "+" : "",
				ingredient.GetName(),
				" ",
				num.ToString(),
				" / ",
				ingredient.req.ToString()
			});
		}
		return text;
	}

	public override bool CanAbandon
	{
		get
		{
			return true;
		}
	}

	[JsonProperty]
	public string idRecipe;

	public RecipeSource _recipe;

	public List<Recipe.Ingredient> ingredients;
}
