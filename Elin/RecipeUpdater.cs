using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RecipeUpdater : EClass
{
	public enum Mode
	{
		Passive,
		Immediate,
		Validate
	}

	public IEnumerator Enumerator;

	public Point pos = new Point();

	public static List<Thing> listD = new List<Thing>();

	public static List<Thing> lastListD = new List<Thing>();

	public static HashSet<Recipe> recipes = new HashSet<Recipe>();

	public static HashSet<Recipe> lastRecipes = new HashSet<Recipe>();

	public static HashSet<string> factories = new HashSet<string>();

	public static Mode mode;

	public static int sync;

	public static bool dirty;

	public void Build(Point _p, Mode _mode = Mode.Passive)
	{
		if (!LayerCraftFloat.Instance)
		{
			return;
		}
		pos.Set(_p);
		mode = _mode;
		sync++;
		Enumerator = RunRecipe().GetEnumerator();
		if (mode == Mode.Passive)
		{
			return;
		}
		for (int i = 0; i < 1000; i++)
		{
			FixedUpdate();
			if (Enumerator == null)
			{
				break;
			}
		}
	}

	public void FixedUpdate()
	{
		if (Enumerator != null && !Enumerator.MoveNext())
		{
			Enumerator = null;
		}
	}

	public IEnumerable RunRecipe()
	{
		recipes.Clear();
		factories.Clear();
		yield return true;
		int count = 0;
		for (int i = pos.x - 1; i < pos.x + 2; i++)
		{
			if (i < 0 || i >= EClass._map.Size)
			{
				continue;
			}
			for (int j = pos.z - 1; j < pos.z + 2; j++)
			{
				if (j < 0 || j >= EClass._map.Size)
				{
					continue;
				}
				Cell cell = EClass._map.cells[i, j];
				if (cell.detail == null || cell.detail.things.Count == 0)
				{
					continue;
				}
				if (pos.cell.effect?.IsFire ?? false)
				{
					factories.Add("fire");
				}
				foreach (Thing thing in cell.detail.things)
				{
					if (thing.IsInstalled)
					{
						if (thing.trait.IsFactory)
						{
							factories.Add(thing.id);
						}
						if (thing.trait.ToggleType == ToggleType.Fire && thing.isOn)
						{
							factories.Add("fire");
						}
					}
				}
			}
		}
		yield return true;
		EClass.pc.things.AddFactory(factories);
		yield return true;
		foreach (Thing thing2 in EClass.pc.things)
		{
			thing2.GetRecipes(recipes);
			count++;
		}
		yield return true;
		for (int k = pos.x - 1; k < pos.x + 2; k++)
		{
			if (k < 0 || k >= EClass._map.Size)
			{
				continue;
			}
			for (int l = pos.z - 1; l < pos.z + 2; l++)
			{
				if (l < 0 || l >= EClass._map.Size)
				{
					continue;
				}
				Cell cell = EClass._map.cells[k, l];
				if (cell.detail == null || cell.detail.things.Count == 0)
				{
					continue;
				}
				foreach (Thing thing3 in cell.detail.things)
				{
					if (thing3.isNPCProperty)
					{
						continue;
					}
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
					thing3.GetRecipes(recipes);
					count++;
				}
			}
		}
		yield return true;
		List<Recipe> list = new List<Recipe>();
		foreach (Recipe recipe in recipes)
		{
			bool flag = true;
			string id = recipe.id;
			if ((id == "waystone" || id == "waystone_temp") && EClass.game.quests.GetPhase<QuestExploration>() >= 6)
			{
				list.Add(recipe);
				continue;
			}
			foreach (Recipe.Ingredient ingredient in recipe.ingredients)
			{
				if (ingredient.thing == null)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				list.Add(recipe);
			}
		}
		foreach (Recipe item in list)
		{
			recipes.Remove(item);
		}
		yield return true;
		if (mode == Mode.Passive && lastRecipes.SetEquals(recipes))
		{
			yield return false;
		}
		else if (mode != Mode.Validate)
		{
			LayerCraftFloat.Instance.RefreshCraft();
		}
		lastRecipes.Clear();
		foreach (Recipe recipe2 in recipes)
		{
			lastRecipes.Add(recipe2);
		}
		yield return null;
	}

	public IEnumerable RunDisassemble()
	{
		lastListD.Clear();
		foreach (Thing item in listD)
		{
			lastListD.Add(item);
		}
		listD.Clear();
		yield return true;
		foreach (Card item2 in EClass.pc.pos.ListCards())
		{
			if (item2.isThing)
			{
				item2.Thing.GetDisassembles(listD);
			}
		}
		yield return true;
		foreach (Thing thing in EClass.pc.things)
		{
			thing.GetDisassembles(listD);
		}
		yield return true;
		if (mode == Mode.Passive && listD.SequenceEqual(lastListD))
		{
			yield return false;
		}
		else if (mode != Mode.Validate)
		{
			LayerCraftFloat.Instance.RefreshDisassemble();
		}
		yield return null;
	}
}
