using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RecipeUpdater : EClass
{
	public void Build(Point _p, RecipeUpdater.Mode _mode = RecipeUpdater.Mode.Passive)
	{
		if (!LayerCraftFloat.Instance)
		{
			return;
		}
		this.pos.Set(_p);
		RecipeUpdater.mode = _mode;
		RecipeUpdater.sync++;
		this.Enumerator = this.RunRecipe().GetEnumerator();
		if (RecipeUpdater.mode != RecipeUpdater.Mode.Passive)
		{
			for (int i = 0; i < 1000; i++)
			{
				this.FixedUpdate();
				if (this.Enumerator == null)
				{
					break;
				}
			}
		}
	}

	public void FixedUpdate()
	{
		if (this.Enumerator == null)
		{
			return;
		}
		if (!this.Enumerator.MoveNext())
		{
			this.Enumerator = null;
		}
	}

	public IEnumerable RunRecipe()
	{
		RecipeUpdater.recipes.Clear();
		RecipeUpdater.factories.Clear();
		yield return true;
		int count = 0;
		for (int i = this.pos.x - 1; i < this.pos.x + 2; i++)
		{
			if (i >= 0 && i < EClass._map.Size)
			{
				for (int j = this.pos.z - 1; j < this.pos.z + 2; j++)
				{
					if (j >= 0 && j < EClass._map.Size)
					{
						Cell cell = EClass._map.cells[i, j];
						if (cell.detail != null && cell.detail.things.Count != 0)
						{
							CellEffect effect = this.pos.cell.effect;
							if (effect != null && effect.IsFire)
							{
								RecipeUpdater.factories.Add("fire");
							}
							foreach (Thing thing in cell.detail.things)
							{
								if (thing.IsInstalled)
								{
									if (thing.trait.IsFactory)
									{
										RecipeUpdater.factories.Add(thing.id);
									}
									if (thing.trait.ToggleType == ToggleType.Fire && thing.isOn)
									{
										RecipeUpdater.factories.Add("fire");
									}
								}
							}
						}
					}
				}
			}
		}
		yield return true;
		EClass.pc.things.AddFactory(RecipeUpdater.factories);
		yield return true;
		foreach (Thing thing2 in EClass.pc.things)
		{
			thing2.GetRecipes(RecipeUpdater.recipes);
			int num = count;
			count = num + 1;
		}
		yield return true;
		for (int k = this.pos.x - 1; k < this.pos.x + 2; k++)
		{
			if (k >= 0 && k < EClass._map.Size)
			{
				for (int l = this.pos.z - 1; l < this.pos.z + 2; l++)
				{
					if (l >= 0 && l < EClass._map.Size)
					{
						Cell cell = EClass._map.cells[k, l];
						if (cell.detail != null && cell.detail.things.Count != 0)
						{
							foreach (Thing thing3 in cell.detail.things)
							{
								if (!thing3.isNPCProperty)
								{
									if (thing3.trait.IsContainer)
									{
										if (!thing3.IsInstalled)
										{
											continue;
										}
									}
									else if (thing3.IsInstalled)
									{
										continue;
									}
									thing3.GetRecipes(RecipeUpdater.recipes);
									int num = count;
									count = num + 1;
								}
							}
						}
					}
				}
			}
		}
		yield return true;
		List<Recipe> list = new List<Recipe>();
		foreach (Recipe recipe in RecipeUpdater.recipes)
		{
			bool flag = true;
			string id = recipe.id;
			if ((id == "waystone" || id == "waystone_temp") && EClass.game.quests.GetPhase<QuestExploration>() >= 6)
			{
				list.Add(recipe);
			}
			else
			{
				using (List<Recipe.Ingredient>.Enumerator enumerator3 = recipe.ingredients.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						if (enumerator3.Current.thing == null)
						{
							flag = false;
							break;
						}
					}
				}
				if (!flag)
				{
					list.Add(recipe);
				}
			}
		}
		foreach (Recipe item in list)
		{
			RecipeUpdater.recipes.Remove(item);
		}
		yield return true;
		if (RecipeUpdater.mode == RecipeUpdater.Mode.Passive && RecipeUpdater.lastRecipes.SetEquals(RecipeUpdater.recipes))
		{
			yield return false;
		}
		else if (RecipeUpdater.mode != RecipeUpdater.Mode.Validate)
		{
			LayerCraftFloat.Instance.RefreshCraft();
		}
		RecipeUpdater.lastRecipes.Clear();
		foreach (Recipe item2 in RecipeUpdater.recipes)
		{
			RecipeUpdater.lastRecipes.Add(item2);
		}
		yield return null;
		yield break;
	}

	public IEnumerable RunDisassemble()
	{
		RecipeUpdater.lastListD.Clear();
		foreach (Thing item in RecipeUpdater.listD)
		{
			RecipeUpdater.lastListD.Add(item);
		}
		RecipeUpdater.listD.Clear();
		yield return true;
		foreach (Card card in EClass.pc.pos.ListCards(false))
		{
			if (card.isThing)
			{
				card.Thing.GetDisassembles(RecipeUpdater.listD);
			}
		}
		yield return true;
		foreach (Thing thing in EClass.pc.things)
		{
			thing.GetDisassembles(RecipeUpdater.listD);
		}
		yield return true;
		if (RecipeUpdater.mode == RecipeUpdater.Mode.Passive && RecipeUpdater.listD.SequenceEqual(RecipeUpdater.lastListD))
		{
			yield return false;
		}
		else if (RecipeUpdater.mode != RecipeUpdater.Mode.Validate)
		{
			LayerCraftFloat.Instance.RefreshDisassemble();
		}
		yield return null;
		yield break;
	}

	public IEnumerator Enumerator;

	public Point pos = new Point();

	public static List<Thing> listD = new List<Thing>();

	public static List<Thing> lastListD = new List<Thing>();

	public static HashSet<Recipe> recipes = new HashSet<Recipe>();

	public static HashSet<Recipe> lastRecipes = new HashSet<Recipe>();

	public static HashSet<string> factories = new HashSet<string>();

	public static RecipeUpdater.Mode mode;

	public static int sync;

	public static bool dirty;

	public enum Mode
	{
		Passive,
		Immediate,
		Validate
	}
}
