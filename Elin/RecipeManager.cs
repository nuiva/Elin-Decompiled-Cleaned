using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class RecipeManager : EClass
{
	public static void BuildList()
	{
		if (!RecipeManager.rebuild && RecipeManager.list.Count > 0)
		{
			return;
		}
		Debug.Log("Rebuilding recipe list");
		RecipeManager.list.Clear();
		RecipeManager.dict.Clear();
		foreach (CardRow cardRow in EClass.sources.cards.rows)
		{
			if (!cardRow.isOrigin && (cardRow.factory.IsEmpty() || !(cardRow.factory[0] == "x")))
			{
				RecipeManager.Create(cardRow, "", cardRow.isChara ? "-c" : "");
			}
		}
		foreach (SourceBlock.Row row in EClass.sources.blocks.rows)
		{
			RecipeManager.Create(row, "Block", "");
			if (row.tileType == TileType.Pillar)
			{
				RecipeManager.Create(row, "BridgePillar", "-p");
			}
		}
		foreach (SourceFloor.Row row2 in EClass.sources.floors.rows)
		{
			if (!row2.tag.Contains("noFloor"))
			{
				RecipeManager.Create(row2, "Floor", "");
			}
		}
		foreach (SourceFloor.Row row3 in EClass.sources.floors.rows)
		{
			if (!row3.tag.Contains("noBridge") && !row3.factory.IsEmpty() && row3.factory[0] != "x")
			{
				RecipeManager.Create(row3, "Bridge", "-b");
			}
		}
		foreach (SourceObj.Row row4 in EClass.sources.objs.rows)
		{
			RecipeManager.Create(row4, "Obj", "");
		}
		foreach (SourceCellEffect.Row row5 in EClass.sources.cellEffects.rows)
		{
			RecipeManager.Create(row5, "Liquid", "");
		}
		RecipeManager.rebuild = false;
	}

	public static void Create(RenderRow row, string type, string suffix = "")
	{
		RecipeSource recipeSource = new RecipeSource();
		recipeSource.id = row.RecipeID + suffix;
		recipeSource.isBridge = (type == "Bridge");
		recipeSource.isBridgePillar = (type == "BridgePillar");
		recipeSource.type = type;
		recipeSource.row = row;
		recipeSource.isChara = (row is SourceChara.Row);
		RecipeManager.list.Add(recipeSource);
		RecipeManager.dict[recipeSource.id] = recipeSource;
		string[] components = row.components;
		Recipe.Create(recipeSource, -1, null).BuildIngredientList();
	}

	public static RecipeSource Get(string id)
	{
		return RecipeManager.dict.TryGetValue(id, null);
	}

	public RecipeSource GetSource(string id)
	{
		return RecipeManager.dict.TryGetValue(id, null);
	}

	public void Add(string id, bool showEffect = true)
	{
		if (id.IsEmpty())
		{
			return;
		}
		RecipeSource recipeSource = RecipeManager.Get(id);
		if (recipeSource == null)
		{
			return;
		}
		if (!this.knownRecipes.ContainsKey(id))
		{
			this.newRecipes.Add(id);
			this.knownRecipes[id] = 0;
		}
		Dictionary<string, int> dictionary = this.knownRecipes;
		int num = dictionary[id];
		dictionary[id] = num + 1;
		int num2 = this.knownRecipes[id];
		if (showEffect)
		{
			EClass.pc.PlaySound("idea", 1f, true);
			EClass.pc.ShowEmo(Emo.idea, 0.5f, false);
		}
		EClass.pc.Say("learnRecipe" + ((num2 == 1) ? "New" : ""), RecipeManager.dict[id].Name.ToTitleCase(false), num2.ToString() ?? "");
		if (recipeSource.row.category == "floor")
		{
			recipeSource = RecipeManager.Get(id + "-b");
			if (recipeSource != null && !this.knownRecipes.ContainsKey(recipeSource.id))
			{
				this.Add(recipeSource.id, false);
			}
			recipeSource = RecipeManager.Get(id.Replace("-b", ""));
			if (recipeSource != null && !this.knownRecipes.ContainsKey(recipeSource.id))
			{
				this.Add(recipeSource.id, false);
			}
		}
	}

	public bool IsKnown(string id)
	{
		return EClass.debug.allRecipe || EClass.player.recipes.knownRecipes.ContainsKey(id);
	}

	public List<RecipeSource> ListSources(Thing factory, List<RecipeSource> newRecipes = null)
	{
		RecipeManager.BuildList();
		List<RecipeSource> list = new List<RecipeSource>();
		foreach (RecipeSource recipeSource in RecipeManager.list)
		{
			if (!recipeSource.isBridgePillar && (factory != null || !(recipeSource.idFactory != "self")) && (factory == null || factory.trait.Contains(recipeSource)) && !recipeSource.isChara && !recipeSource.noListing)
			{
				if (!EClass.debug.godCraft && !EClass.player.recipes.knownRecipes.ContainsKey(recipeSource.id))
				{
					bool flag = false;
					if (recipeSource.row.recipeKey != null && recipeSource.row.recipeKey.Length != 0 && recipeSource.row.recipeKey[0][0] == '*')
					{
						flag = true;
					}
					string id = recipeSource.id;
					if (!(id == "waystone"))
					{
						if (id == "container_shipping")
						{
							if (EClass.game.quests.GetPhase<QuestShippingChest>() >= 0)
							{
								flag = true;
							}
						}
					}
					else if (EClass.game.quests.GetPhase<QuestExploration>() >= 6)
					{
						flag = true;
					}
					if (!flag)
					{
						continue;
					}
					if (newRecipes != null)
					{
						EClass.player.recipes.Add(recipeSource.id, true);
						if (EClass.player.recipes.newRecipes.Contains(recipeSource.id))
						{
							newRecipes.Add(recipeSource);
							EClass.player.recipes.newRecipes.Remove(recipeSource.id);
						}
					}
				}
				list.Add(recipeSource);
			}
		}
		return list;
	}

	public void OnSleep(bool ehe = false)
	{
		int slept = EClass.player.stats.slept;
		Rand.SetSeed(EClass.game.seed + slept);
		if (slept > 3 && EClass.rnd(ehe ? 777 : 3) == 0)
		{
			return;
		}
		if (EClass.rnd(EClass.pc.Evalue(1642) + 1) <= 0)
		{
			if (slept > 15 && EClass.rnd(3) == 0)
			{
				return;
			}
			if (slept > 30 && EClass.rnd(3) == 0)
			{
				return;
			}
			if (slept > 60 && EClass.rnd(3) == 0)
			{
				return;
			}
		}
		Msg.Say("learnRecipeSleep");
		Rand.SetSeed(-1);
		this.Add(RecipeManager.GetRandomRecipe((ehe ? 5 : 0) + EClass.rnd(EClass.rnd(EClass.rnd(10))), null, false), true);
	}

	public void ComeUpWithRandomRecipe(string idCat = null, int lvBonus = 0)
	{
		string randomRecipe = RecipeManager.GetRandomRecipe(lvBonus, idCat, true);
		if (randomRecipe == null)
		{
			return;
		}
		Msg.Say("learnRecipeIdea");
		this.Add(randomRecipe, true);
	}

	public void ComeUpWithRecipe(string idRecipe, int chanceForRandomRecipe = 0)
	{
		if (idRecipe.IsEmpty())
		{
			return;
		}
		RecipeSource recipeSource = RecipeManager.Get(idRecipe);
		if (EClass.rnd(10) != 0 && !EClass.debug.enable)
		{
			return;
		}
		if (recipeSource == null || EClass.player.recipes.knownRecipes.ContainsKey(idRecipe) || (!recipeSource.NeedFactory && !recipeSource.IsQuickCraft))
		{
			if (recipeSource != null && chanceForRandomRecipe > 0 && EClass.rnd(EClass.debug.enable ? 1 : chanceForRandomRecipe) == 0)
			{
				this.ComeUpWithRandomRecipe(recipeSource.row.Category.id, 0);
			}
			return;
		}
		int id = recipeSource.GetReqSkill().id;
		if (EClass.pc.Evalue(id) + 5 < recipeSource.row.LV)
		{
			Msg.Say("recipeReqLv", EClass.sources.elements.map[id].GetName(), null, null, null);
			return;
		}
		Msg.Say("learnRecipeIdea");
		this.Add(idRecipe, true);
	}

	public static RecipeSource GetUnlearnedRecipe(int lvBonus, string cat, bool onlyUnlearned)
	{
		RecipeManager.BuildList();
		List<RecipeSource> list = new List<RecipeSource>();
		foreach (RecipeSource recipeSource in RecipeManager.list)
		{
			if ((recipeSource.NeedFactory || recipeSource.IsQuickCraft) && (cat == null || recipeSource.row.Category.IsChildOf(cat)) && (!onlyUnlearned || !EClass.player.recipes.knownRecipes.ContainsKey(recipeSource.id)) && EClass.pc.Evalue(recipeSource.GetReqSkill().id) + 5 + lvBonus >= recipeSource.row.LV)
			{
				list.Add(recipeSource);
			}
		}
		return list.RandomItemWeighted((RecipeSource r) => (float)((r.row.chance == 0) ? 100 : r.row.chance));
	}

	public static RecipeSource GetLearnedRecipe(string cat = null)
	{
		List<RecipeSource> list = new List<RecipeSource>();
		foreach (string id in EClass.player.recipes.knownRecipes.Keys)
		{
			RecipeSource recipeSource = RecipeManager.Get(id);
			if (recipeSource != null && recipeSource.row is SourceThing.Row && (recipeSource.NeedFactory || recipeSource.IsQuickCraft) && (cat == null || recipeSource.row.Category.IsChildOf(cat)))
			{
				list.Add(recipeSource);
			}
		}
		return list.RandomItemWeighted((RecipeSource r) => (float)((r.row.chance == 0) ? 100 : r.row.chance));
	}

	public static string GetRandomRecipe(int lvBonus, string cat = null, bool onlyUnlearned = false)
	{
		RecipeSource recipeSource = RecipeManager.GetUnlearnedRecipe(lvBonus, cat, onlyUnlearned);
		if (recipeSource == null)
		{
			recipeSource = RecipeManager.GetLearnedRecipe(cat);
		}
		if (recipeSource == null)
		{
			return null;
		}
		return recipeSource.id;
	}

	public void OnVersionUpdate()
	{
		EClass.pc.things.Foreach(delegate(Thing t)
		{
			this.knownIngredients.Add(t.id);
		}, true);
		this.knownIngredients.Add("flower");
	}

	public static bool rebuild;

	public static List<RecipeSource> list = new List<RecipeSource>();

	public static Dictionary<string, RecipeSource> dict = new Dictionary<string, RecipeSource>();

	[JsonProperty]
	public HashSet<string> knownIngredients = new HashSet<string>();

	[JsonProperty]
	public Dictionary<string, int> knownRecipes = new Dictionary<string, int>();

	[JsonProperty]
	public HashSet<string> hoveredRecipes = new HashSet<string>();

	[JsonProperty]
	public HashSet<string> newRecipes = new HashSet<string>();
}
