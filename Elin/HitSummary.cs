using System;
using System.Collections.Generic;
using UnityEngine;

public class HitSummary : EClass
{
	public void Clear()
	{
		this.hasFactory = true;
		this.money = (this.count = (this.countValid = 0));
		this.targets.Clear();
		this.groups.Clear();
	}

	public void SetRecipe(Recipe r)
	{
		this.recipe = r;
		this.hasFactory = true;
		if (r != null && !r.UseStock && r.source.NeedFactory)
		{
			PropSet propSet = EClass._map.Installed.cardMap.TryGetValue(r.source.idFactory, null);
			if (propSet == null || propSet.Count == 0)
			{
				this.hasFactory = false;
			}
		}
	}

	public bool CanExecute()
	{
		if (EClass.debug.ignoreBuildRule)
		{
			return true;
		}
		if (!this.hasFactory || EClass.pc.GetCurrency("money") < this.money)
		{
			return false;
		}
		if (EClass.screen.tileSelector.processing)
		{
			return true;
		}
		if (this.recipe != null)
		{
			foreach (Recipe.Ingredient ingredient in this.recipe.ingredients)
			{
				if (!ingredient.optional)
				{
					if (ingredient.thing == null)
					{
						return false;
					}
					if (ingredient.thing.Num < ingredient.req * this.countValid)
					{
						return false;
					}
				}
			}
			return true;
		}
		return true;
	}

	public void Execute()
	{
		if (!EClass.debug.ignoreBuildRule)
		{
			EClass.pc.ModCurrency(-this.money, "money");
		}
		if (this.recipe == null || (this.recipe.UseStock && !this.recipe.VirtualBlock))
		{
			return;
		}
		if (this.recipe.RequireIngredients)
		{
			BuildMenu.Instance.info1.lastMats[this.recipe.id] = this.recipe.ingredients[0].mat;
		}
		if (this.recipe.tileType.CanInstaComplete || EClass.player.instaComplete)
		{
			foreach (Recipe.Ingredient ingredient in this.recipe.ingredients)
			{
				Thing thing = this.recipe.UseStock ? this.recipe.ingredients[0].thing : ingredient.RefreshThing();
				if (thing == null)
				{
					if (!EClass.debug.enable)
					{
						Debug.LogError("no ing");
					}
					break;
				}
				int num = ingredient.req * this.countValid;
				int num2 = (thing.Num >= num) ? num : thing.Num;
				if (!EClass.debug.godBuild || this.recipe.UseStock)
				{
					thing.ModNum(-num2, true);
				}
			}
		}
	}

	public int money;

	public int count;

	public int countValid;

	public Recipe recipe;

	public List<IInspect> targets = new List<IInspect>();

	public List<InspectGroup> groups = new List<InspectGroup>();

	public Thing factory;

	public bool hasFactory;
}
