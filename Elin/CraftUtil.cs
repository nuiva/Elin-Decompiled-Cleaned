using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CraftUtil : EClass
{
	public static void ModRandomFoodEnc(Thing t)
	{
		List<Element> list = new List<Element>();
		foreach (Element element in t.elements.dict.Values)
		{
			if (element.IsFoodTrait)
			{
				list.Add(element);
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		Element element2 = list.RandomItem<Element>();
		t.elements.ModBase(element2.id, EClass.rnd(6) + 1);
		if (element2.Value > 60)
		{
			t.elements.SetTo(element2.id, 60);
		}
	}

	public static void AddRandomFoodEnc(Thing t)
	{
		List<SourceElement.Row> list = (from e in EClass.sources.elements.rows
		where e.foodEffect.Length > 1 && CraftUtil.ListFoodEffect.Contains(e.foodEffect[0])
		select e).ToList<SourceElement.Row>();
		list.ForeachReverse(delegate(SourceElement.Row e)
		{
			if (t.elements.dict.ContainsKey(e.id))
			{
				list.Remove(e);
			}
		});
		if (list.Count == 0)
		{
			return;
		}
		SourceElement.Row row = list.RandomItemWeighted((SourceElement.Row a) => (float)a.chance);
		t.elements.SetBase(row.id, 1, 0);
		t.c_seed = row.id;
	}

	public static void MakeDish(Thing food, int lv, Chara crafter = null)
	{
		RecipeManager.BuildList();
		List<Thing> list = new List<Thing>();
		RecipeSource recipeSource = RecipeManager.Get(food.id);
		Debug.Log(recipeSource);
		if (recipeSource == null)
		{
			return;
		}
		int num = Mathf.Min(EClass.rnd(lv), 50);
		foreach (Recipe.Ingredient ingredient in recipeSource.GetIngredients())
		{
			Thing thing = ThingGen.Create(ingredient.id, -1, -1);
			TraitSeed.LevelSeed(thing, null, EClass.rnd(lv / 4) + 1);
			thing.SetEncLv(thing.encLV / 2);
			if (num > 0 && EClass.rnd(3) == 0)
			{
				thing.elements.SetBase(2, num, 0);
			}
			list.Add(thing);
		}
		CraftUtil.MakeDish(food, list, num, crafter);
	}

	public static void MakeDish(Card food, List<Thing> ings, int qualityBonus, Chara crafter = null)
	{
		List<Thing> list = new List<Thing>();
		bool flag = food.sourceCard.vals.Contains("fixed");
		for (int i = 0; i < ings.Count; i++)
		{
			Thing thing = ings[i];
			if (flag)
			{
				list.Add(thing);
				break;
			}
			if (!CraftUtil.<MakeDish>g__IsIgnoreName|5_0(thing))
			{
				list.Add(thing);
			}
		}
		if (list.Count > 0)
		{
			Thing thing2 = list.RandomItem<Thing>();
			if (thing2 != null)
			{
				food.MakeRefFrom(thing2, null);
				if (thing2.c_idRefCard != null)
				{
					food.c_idRefCard = thing2.c_idRefCard;
					food.c_altName = food.TryGetFoodName(thing2);
					if (thing2.id == "_egg" || thing2.id == "egg_fertilized")
					{
						food.c_altName = "_egg".lang(food.c_altName, null, null, null, null);
					}
				}
			}
		}
		CraftUtil.MixIngredients(food, ings, CraftUtil.MixType.Food, qualityBonus, crafter);
	}

	public static Thing MixIngredients(string idProduct, List<Thing> ings, CraftUtil.MixType type, int idMat = 0, Chara crafter = null)
	{
		Thing thing = ThingGen.Create(idProduct, -1, -1);
		if (idMat != 0)
		{
			thing.ChangeMaterial(idMat);
		}
		CraftUtil.MixIngredients(thing, ings, type, 999, crafter);
		return thing;
	}

	public static Card MixIngredients(Card product, List<Thing> ings, CraftUtil.MixType type, int maxQuality, Chara crafter = null)
	{
		CraftUtil.<>c__DisplayClass7_0 CS$<>8__locals1;
		CS$<>8__locals1.type = type;
		CS$<>8__locals1.product = product;
		CS$<>8__locals1.isFood = (CS$<>8__locals1.type == CraftUtil.MixType.Food);
		CS$<>8__locals1.nutFactor = 100 - (ings.Count - 1) * 5;
		if (crafter != null && crafter.Evalue(1650) >= 3)
		{
			CS$<>8__locals1.nutFactor -= 10;
		}
		foreach (Element element in CS$<>8__locals1.product.elements.dict.Values)
		{
			int id = element.id;
			if (id - 914 > 1 && element.Value >= 0 && CraftUtil.<MixIngredients>g__IsValidTrait|7_0(element, ref CS$<>8__locals1))
			{
				CS$<>8__locals1.product.elements.SetTo(element.id, 0);
			}
		}
		if (CS$<>8__locals1.isFood)
		{
			CS$<>8__locals1.product.elements.SetTo(10, 5);
		}
		int num = 0;
		int num2 = 0;
		foreach (Thing thing in ings)
		{
			if (thing != null)
			{
				CraftUtil.<MixIngredients>g__MixElements|7_1(thing, ref CS$<>8__locals1);
				if (CS$<>8__locals1.isFood)
				{
					num += Mathf.Clamp(thing.SelfWeight * 80 / 100, 50, 400 + thing.SelfWeight / 20);
					int value = thing.GetValue(false);
					num2 += value;
				}
			}
		}
		if (CS$<>8__locals1.isFood)
		{
			CS$<>8__locals1.product.isWeightChanged = true;
			CS$<>8__locals1.product.c_weight = num;
			CS$<>8__locals1.product.c_priceAdd = num2;
		}
		if (CS$<>8__locals1.product.HasElement(652, 1))
		{
			CS$<>8__locals1.product.ChangeWeight((CS$<>8__locals1.isFood ? num : CS$<>8__locals1.product.Thing.source.weight) * 100 / (100 + CS$<>8__locals1.product.Evalue(652)));
		}
		if (CS$<>8__locals1.product.elements.Value(2) > maxQuality)
		{
			CS$<>8__locals1.product.elements.SetTo(2, maxQuality);
		}
		if (CS$<>8__locals1.product.id == "map")
		{
			int num3 = 1 + CS$<>8__locals1.product.Evalue(2) + CS$<>8__locals1.product.Evalue(751);
			if (num3 < 1)
			{
				num3 = 1;
			}
			foreach (Thing thing2 in ings)
			{
				if (thing2 != null && thing2.Thing != null && !(thing2.id != "gem"))
				{
					num3 *= thing2.Thing.material.hardness / 20 + 2;
				}
			}
			if (num3 > EClass.player.stats.deepest + 10 - 1)
			{
				num3 = EClass.player.stats.deepest + 10 - 1;
			}
			CS$<>8__locals1.product.SetInt(25, num3);
		}
		return CS$<>8__locals1.product;
	}

	[CompilerGenerated]
	internal static bool <MakeDish>g__IsIgnoreName|5_0(Card t)
	{
		if (t == null)
		{
			return true;
		}
		string origin = t.sourceCard._origin;
		return origin == "dough" || origin == "dish" || origin == "dish_lunch";
	}

	[CompilerGenerated]
	internal static bool <MixIngredients>g__IsValidTrait|7_0(Element e, ref CraftUtil.<>c__DisplayClass7_0 A_1)
	{
		CraftUtil.MixType type = A_1.type;
		if (type != CraftUtil.MixType.General)
		{
			if (type == CraftUtil.MixType.Food)
			{
				if (e.IsFoodTrait || e.IsTrait || e.id == 2)
				{
					return true;
				}
			}
		}
		else
		{
			if (e.IsTrait)
			{
				return true;
			}
			if (e.IsFoodTrait)
			{
				return A_1.product.ShowFoodEnc;
			}
		}
		return false;
	}

	[CompilerGenerated]
	internal static void <MixIngredients>g__MixElements|7_1(Card t, ref CraftUtil.<>c__DisplayClass7_0 A_1)
	{
		if (t == null)
		{
			return;
		}
		foreach (Element element in t.elements.dict.Values)
		{
			if (CraftUtil.<MixIngredients>g__IsValidTrait|7_0(element, ref A_1))
			{
				if (A_1.isFood && element.IsFoodTraitMain)
				{
					A_1.product.elements.ModBase(element.id, element.Value);
				}
				else
				{
					int num = A_1.product.elements.Base(element.id);
					if ((num <= 0 && element.Value < 0 && element.Value < num) || (element.Value > 0 && element.Value > num))
					{
						A_1.product.elements.SetTo(element.id, element.Value);
					}
				}
			}
		}
		if (A_1.isFood)
		{
			A_1.product.elements.ModBase(10, t.Evalue(10) * A_1.nutFactor / 100);
		}
	}

	public static string[] ListFoodEffect = new string[]
	{
		"exp",
		"pot"
	};

	public enum MixType
	{
		General,
		Food
	}
}
