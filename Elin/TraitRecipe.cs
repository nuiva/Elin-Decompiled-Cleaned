using System;

public class TraitRecipe : TraitScroll
{
	public override string IdNoRestock
	{
		get
		{
			return this.owner.id + "_" + this.ID;
		}
	}

	public string ID
	{
		get
		{
			return this.owner.GetStr(53, null);
		}
	}

	public override bool CanBeShipped
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public RecipeSource recipe
	{
		get
		{
			return RecipeManager.dict.TryGetValue(this.ID, null) ?? RecipeManager.dict.TryGetValue("bait", null);
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.SetStr(53, RecipeManager.GetRandomRecipe(lv, null, false));
	}

	public override void OnRead(Chara c)
	{
		EClass.player.recipes.Add(this.recipe.id, true);
		this.owner.ModNum(-1, true);
	}

	public override void SetName(ref string s)
	{
		int num = EClass.player.recipes.knownRecipes.TryGetValue(this.ID, 0);
		s = "_recipe".lang(this.recipe.Name.ToTitleCase(false), s, null, null, null) + ((num == 0) ? "" : "recipe_learnt".lang(num.ToString() ?? "", null, null, null, null));
	}

	public override bool CanStackTo(Thing to)
	{
		return this.ID == to.GetStr(53, null);
	}

	public override void WriteNote(UINote n, bool identified)
	{
		if (this.recipe.NeedFactory)
		{
			n.AddText("isCraftedAt".lang(this.recipe.NameFactory.ToTitleCase(true), null, null, null, null), FontColor.DontChange);
		}
	}

	public override int GetValue()
	{
		return this.owner.sourceCard.value * (100 + this.recipe.row.LV * 15) / 100;
	}
}
