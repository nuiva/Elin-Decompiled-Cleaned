using System;
using System.Collections.Generic;

public class RecipeSource : EClass
{
	public string recipeCat
	{
		get
		{
			if (!this.isBridge && !this.isBridgePillar)
			{
				return this.row.Category.recipeCat;
			}
			return "foundation";
		}
	}

	public string GetDetail()
	{
		return this.row.GetDetail();
	}

	public string Name
	{
		get
		{
			return this.row.GetName() + (this.isBridge ? "recipeBridge".lang() : "");
		}
	}

	public bool IsQuickCraft
	{
		get
		{
			return !this.row.factory.IsEmpty() && this.row.factory[0] == "self";
		}
	}

	public bool NeedFactory
	{
		get
		{
			return !this.row.factory.IsEmpty() && this.row.factory[0] != "self" && this.row.factory[0] != "x" && this.row.factory[0] != "none" && this.row.factory[0] != "None";
		}
	}

	public string NameFactory
	{
		get
		{
			return EClass.sources.cards.map[this.idFactory].GetName();
		}
	}

	public string idFactory
	{
		get
		{
			if (this.row.factory.IsEmpty())
			{
				return null;
			}
			if (this.isBridge)
			{
				return "factory_platform";
			}
			if (!this.isBridgePillar)
			{
				return this.row.factory[0];
			}
			return "tool_carving";
		}
	}

	public Element GetReqSkill()
	{
		return Element.Create((this.NeedFactory ? EClass.sources.cards.GetModelCrafter(this.idFactory) : Trait.SelfFactory).IDReqEle(this), this.row.LV);
	}

	public List<Recipe.Ingredient> GetIngredients()
	{
		if (!this.row.factory.IsEmpty() && this.row.factory[0] == "x")
		{
			this.noListing = true;
		}
		if (this.row.components.Length == 0)
		{
			return RecipeSource.DefaultIngredients;
		}
		if (this.row.components[0] == "-")
		{
			return RecipeSource.DefaultIngredients;
		}
		if (this.row.recipeKey.Length != 0 && this.row.recipeKey[0] == "*")
		{
			this.alwaysKnown = true;
		}
		string[] components = this.row.components;
		List<Recipe.Ingredient> list = new List<Recipe.Ingredient>();
		if (!components.IsEmpty() && components[0] != "-")
		{
			int i = 0;
			while (i < components.Length)
			{
				string[] array = components[i].Split('|', StringSplitOptions.None);
				string[] array2 = array[0].Split('/', StringSplitOptions.None);
				string[] array3 = array2[0].Split('@', StringSplitOptions.None);
				bool optional = false;
				bool useCat = false;
				for (;;)
				{
					char c = array3[0][0];
					if (c == '#')
					{
						goto IL_12E;
					}
					if (c == '$')
					{
						goto IL_116;
					}
					if (c != '+')
					{
						break;
					}
					optional = true;
					array3[0] = array3[0].Remove(0, 1);
				}
				IL_156:
				Recipe.Ingredient ingredient = new Recipe.Ingredient
				{
					id = array3[0],
					tag = ((array3.Length > 1) ? array3[1] : null),
					req = ((array2.Length > 1) ? int.Parse(array2[1]) : 1),
					optional = optional,
					useCat = useCat
				};
				if (array.Length > 1)
				{
					for (int j = 1; j < array.Length; j++)
					{
						ingredient.idOther.Add(array[j]);
					}
				}
				list.Add(ingredient);
				i++;
				continue;
				goto IL_156;
				IL_116:
				this.colorIng = i;
				array3[0] = array3[0].Remove(0, 1);
				goto IL_156;
				IL_12E:
				useCat = true;
				array3[0] = array3[0].Remove(0, 1);
				goto IL_156;
			}
		}
		return list;
	}

	public string GetIDIngredient()
	{
		string[] components = this.row.components;
		if (!components.IsEmpty() && components[0] != "-")
		{
			int num = 0;
			if (num < components.Length)
			{
				string[] array = components[num].Split('/', StringSplitOptions.None)[0].Split('@', StringSplitOptions.None);
				char c = array[0][0];
				if (c == '#' || c == '$' || c == '+')
				{
					array[0] = array[0].Remove(0, 1);
				}
				return array[0];
			}
		}
		return null;
	}

	public int GetSPCost(Card factory)
	{
		Element reqSkill = this.GetReqSkill();
		int num = this.row.Category.costSP + reqSkill.Value / 10;
		int num2 = EClass.pc.Evalue(reqSkill.id);
		if (num2 < reqSkill.Value)
		{
			num += (reqSkill.Value - num2) * 2 / 3;
		}
		return num;
	}

	public RenderRow row;

	public string type;

	public string id;

	public int colorIng;

	public bool isBridge;

	public bool isBridgePillar;

	public bool isChara;

	public bool noListing;

	public bool isRandom;

	public bool alwaysKnown;

	public static List<Recipe.Ingredient> DefaultIngredients = new List<Recipe.Ingredient>
	{
		new Recipe.Ingredient
		{
			id = "log",
			req = 1
		}
	};
}
