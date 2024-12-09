using System.Collections.Generic;
using UnityEngine;

public class FoodEffect : EClass
{
	public static bool IsHumanFlesh(Thing food)
	{
		if (food.HasTag(CTAG.notHumanMeat))
		{
			return false;
		}
		if (!IsHumanFlesh(food.refCard))
		{
			return IsHumanFlesh(food.refCard2);
		}
		return true;
	}

	public static bool IsUndeadFlesh(Thing food)
	{
		if (!IsUndeadFlesh(food.refCard))
		{
			return IsUndeadFlesh(food.refCard2);
		}
		return true;
	}

	public static bool IsHumanFlesh(CardRow r)
	{
		if (r == null)
		{
			return false;
		}
		if (r.id == "chara")
		{
			return EClass.pc.race.tag.Contains("human");
		}
		if (r.isChara)
		{
			return EClass.sources.races.map[EClass.sources.charas.map[r.id].race].tag.Contains("human");
		}
		return false;
	}

	public static bool IsUndeadFlesh(CardRow r)
	{
		if (r == null)
		{
			return false;
		}
		if (r.id == "chara")
		{
			return EClass.pc.race.tag.Contains("undead");
		}
		if (r.isChara)
		{
			return EClass.sources.races.map[EClass.sources.charas.map[r.id].race].tag.Contains("undead");
		}
		return false;
	}

	public static void Proc(Chara eater, Thing food)
	{
		bool isEaterAtDiningSpot = EClass._zone.IsPCFaction && eater.IsInSpot<TraitSpotDining>();
		int featGourmet3Bonus = (food.isCrafted ? ((EClass.pc.Evalue(1650) >= 3) ? 5 : 0) : 0);
		float foodEffectMultiplier = (float)(100 + (isEaterAtDiningSpot ? 10 : 0) + featGourmet3Bonus + Mathf.Min(food.QualityLv * 10, 100)) / 200f;
		if (foodEffectMultiplier < 0.1f)
		{
			foodEffectMultiplier = 0.1f;
		}
		int foodNutrition = food.Evalue(10 /* nutrition */);
		float foodTasteIndicator = 40f;
		float foodTasteIndicatorMultiplier = 1f;
		string foodTasteLangId = "";
		bool isFoodHumanFlesh = IsHumanFlesh(food);
		bool isFoodUndeadFlesh = IsUndeadFlesh(food);
		if (food.source._origin != "meat" && food.source._origin != "dish")
		{
			isFoodHumanFlesh = (isFoodUndeadFlesh = false);
		}
		if (food.id == "deadbody")
		{
			isFoodHumanFlesh = true;
		}
		string[] components = food.source.components;
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i].Contains("egg"))
			{
				isFoodHumanFlesh = false;
			}
		}
		bool isEaterCannibal = eater.HasElement(1205 /* featCannibalism */);
		bool isFoodDecayed = food.IsDecayed || isFoodUndeadFlesh;
		if (food.IsBlessed)
		{
			foodEffectMultiplier *= 1.5f;
		}
		if (food.IsCursed)
		{
			foodEffectMultiplier *= 0.5f;
		}
		if (isEaterCannibal)
		{
			if (isFoodHumanFlesh)
			{
				foodTasteIndicatorMultiplier *= 2f;
				foodEffectMultiplier *= 1.5f;
			}
			else
			{
				foodTasteIndicatorMultiplier *= 0.5f;
				foodEffectMultiplier /= 2f;
				foodNutrition /= 2;
			}
		}
		else if (isFoodHumanFlesh)
		{
			foodTasteIndicatorMultiplier = 0f;
			foodEffectMultiplier *= 0.5f;
		}
		if (eater.HasElement(1200))
		{
			foodEffectMultiplier *= 1.25f;
		}
		if (!eater.IsPC)
		{
			foodEffectMultiplier *= 3f;
		}
		if (isFoodDecayed && !eater.HasElement(480))
		{
			if (eater.IsPC)
			{
				if (isFoodUndeadFlesh)
				{
					eater.Say("food_undead");
				}
				eater.Say("food_rot");
			}
			foodTasteIndicatorMultiplier = 0f;
			foodNutrition /= 2;
		}
		else
		{
			switch (food.source._origin)
			{
			case "meat":
				if (eater.IsPC)
				{
					eater.Say("food_raw_meat");
				}
				foodEffectMultiplier *= 0.7f;
				foodTasteIndicatorMultiplier = 0.5f;
				break;
			case "fish":
				if (eater.IsHuman)
				{
					if (eater.IsPC)
					{
						eater.Say("food_raw_fish");
					}
					foodEffectMultiplier *= 0.9f;
					foodTasteIndicatorMultiplier = 0.5f;
				}
				break;
			case "dough":
				if (eater.IsPC)
				{
					eater.Say("food_raw_powder");
				}
				foodEffectMultiplier *= 0.9f;
				foodTasteIndicatorMultiplier = 0.5f;
				break;
			}
		}
		float foodEffectHungerMultiplier = Mathf.Min(eater.hunger.value, foodNutrition);
		if (eater.hunger.GetPhase() >= 3)
		{
			foodEffectHungerMultiplier *= 1.1f;
		}
		if (isFoodDecayed && !eater.HasElement(480 /* strongStomach */))
		{
			eater.ModExp(70 /* STR */, -300);
			eater.ModExp(71 /* END */, -300);
			eater.ModExp(72 /* DEX */, -200);
			eater.ModExp(73 /* PER */, -200);
			eater.ModExp(74 /* LER */, -200);
			eater.ModExp(75 /* WIL */, 500);
			eater.ModExp(76 /* MAG */, -200);
			eater.ModExp(77 /* CHA */, -300);
		}
		else
		{
			foodEffectMultiplier = foodEffectMultiplier * foodEffectHungerMultiplier / 10f;
			if (eater.HasCondition<ConAnorexia>())
			{
				foodEffectMultiplier = 0.01f;
			}
			List<Element> list = food.ListValidTraits(isCraft: true, limit: false);
			foreach (Element value in food.elements.dict.Values) // Apply food effects
			{
				if (value.source.foodEffect.IsEmpty() || !list.Contains(value))
				{
					continue;
				}
				string[] foodEffect = value.source.foodEffect;
				int id = value.id;
				float foodEffectMultiplier2 = foodEffectMultiplier * (float)value.Value;
				if (value.source.category == "food" && eater.IsPC)
				{
					bool isEffectPositive = foodEffectMultiplier2 >= 0f;
					string foodEffectText = value.source.GetText(isEffectPositive ? "textInc" : "textDec", returnNull: true);
					if (foodEffectText != null)
					{
						Msg.SetColor(isEffectPositive ? "positive" : "negative");
						eater.Say(foodEffectText);
					}
				}
				switch (foodEffect[0])
				{
				case "exp":
				{
					id = ((foodEffect.Length > 1) ? EClass.sources.elements.alias[foodEffect[1]].id : value.id);
					int expGain = (int)(foodEffectMultiplier2 * (float)((foodEffect.Length > 2) ? foodEffect[2].ToInt() : 4)) * 2 / 3;
					eater.ModExp(id, expGain);
					break;
				}
				case "pot":
				{
					id = ((foodEffect.Length > 1) ? EClass.sources.elements.alias[foodEffect[1]].id : value.id);
					int eaterCurrentBonusPotential = eater.elements.GetElement(id).vTempPotential;
					int potentialGain = EClass.rndHalf((int)(foodEffectMultiplier2 / 5f) + 1);
					potentialGain = potentialGain * 100 / Mathf.Max(100, eaterCurrentBonusPotential * 2 / 3);
					eater.elements.ModTempPotential(id, potentialGain, 8);
					break;
				}
				case "karma":
					if (eater.IsPCParty)
					{
						EClass.player.ModKarma(-5);
					}
					break;
				case "poison":
					ActEffect.Poison(eater, EClass.pc, value.Value * 10);
					if (eater.isDead)
					{
						return;
					}
					break;
				case "love":
					ActEffect.LoveMiracle(eater, EClass.pc, value.Value * 10);
					break;
				case "loseWeight":
					eater.ModWeight(-EClass.rndHalf(value.Value), ignoreLimit: true);
					break;
				case "gainWeight":
					eater.ModWeight(EClass.rndHalf(value.Value), ignoreLimit: true);
					break;
				case "little":
				{
					int numSistersEaten = eater.GetInt(112);
					if (numSistersEaten >= 30)
					{
						break;
					}
					eater.Say("little_eat", eater);
					eater.PlaySound("ding_potential");
					int potentialGain = Mathf.Max(5 - numSistersEaten / 2, 1);
					Debug.Log("sister eaten:" + numSistersEaten + "/" + potentialGain);
					foreach (Element value2 in eater.elements.dict.Values) // Add potential to main stats
					{
						if (value2.IsMainAttribute)
						{
							eater.elements.ModPotential(value2.id, potentialGain);
						}
					}
					if (eater.race.id == "mutant" && eater.elements.Base(1230 /* featAdam */) < 10) // Add Adam feat
					{
						eater.Say("little_adam", eater);
						eater.SetFeat(1230, eater.elements.Base(1230 /* featAdam */) + 1);
					}
					eater.SetInt(112, numSistersEaten + 1);
					break;
				}
				}
			}
		}
		ProcTrait(eater, food);
		foodTasteIndicator += (float)food.Evalue(70 /* STR */);
		foodTasteIndicator += (float)(food.Evalue(72 /* DEX */) / 2);
		foodTasteIndicator += (float)(food.Evalue(73 /* PER */) / 2);
		foodTasteIndicator += (float)(food.Evalue(75 /* WIL */) / 2);
		foodTasteIndicator += (float)(food.Evalue(76 /* MAG */) * 3 / 2);
		foodTasteIndicator += (float)food.Evalue(440 /* sustain_STR */);
		foodTasteIndicator += (float)(food.Evalue(445 /* sustain_WIL */) / 2);
		foodTasteIndicator -= (float)food.Evalue(71 /* END */);
		foodTasteIndicator -= (float)(foodNutrition / 2);
		foodTasteIndicator *= foodTasteIndicatorMultiplier;
		if (foodTasteLangId.IsEmpty())
		{
			if (foodTasteIndicator > 100f)
			{
				foodTasteLangId = "food_great";
			}
			else if (foodTasteIndicator > 70f)
			{
				foodTasteLangId = "food_good";
			}
			else if (foodTasteIndicator > 50f)
			{
				foodTasteLangId = "food_soso";
			}
			else if (foodTasteIndicator > 30f)
			{
				foodTasteLangId = "food_average";
			}
			else
			{
				foodTasteLangId = "food_bad";
			}
			if (eater.IsPC)
			{
				eater.Say(foodTasteLangId);
				if (isFoodHumanFlesh)
				{
					eater.Say(isEaterCannibal ? "food_human_pos" : "food_human_neg");
				}
				else if (isEaterCannibal)
				{
					eater.Say("food_human_whine");
				}
			}
		}
		if (LangGame.Has(foodTasteLangId + "2"))
		{
			eater.Say(foodTasteLangId + "2", eater, food);
		}
		if (!eater.IsPCParty)
		{
			foodNutrition *= 2;
		}
		foodNutrition = foodNutrition * (100 + eater.Evalue(1235 /* featLightEater */) * 10) / (100 + eater.Evalue(1234 /* featHeavyEater */) * 10 + eater.Evalue(1236 /* featNorland */) * 15);
		eater.hunger.Mod(-foodNutrition);
		if (isFoodHumanFlesh) // Empower cannibalism traits for eater
		{
			if (!isEaterCannibal)
			{
				if (eater.IsHuman)
				{
					eater.AddCondition<ConInsane>(200);
					eater.SAN.Mod(15);
				}
				if (EClass.rnd(eater.IsHuman ? 5 : 20) == 0)
				{
					eater.SetFeat(1205 /* featCannibalism */, 1, msg: true);
					isEaterCannibal = true;
				}
			}
			if (isEaterCannibal)
			{
				eater.SetInt(31, EClass.world.date.GetRaw() + 10080);
			}
		}
		else if (isEaterCannibal && eater.GetInt(31) < EClass.world.date.GetRaw()) // Weaken cannibalism traits for eater
		{
			eater.SetFeat(1205 /* featCannibalism */, 0, msg: true);
		}
		if (isFoodDecayed && !eater.HasElement(480 /* strongStomach */))
		{
			eater.AddCondition<ConParalyze>();
			eater.AddCondition<ConConfuse>(200);
		}
		if (eater.HasCondition<ConAnorexia>())
		{
			eater.Vomit();
		}
		if (foodNutrition > 20 && eater.HasElement(1413 /* featFairysan */)) // Spit out seeds after eating
		{
			Thing thing = ThingGen.Create("seed");
			if (EClass.rnd(EClass.debug.enable ? 1 : 10) == 0)
			{
				TraitSeed.ApplySeed(thing, 90);
			}
			thing.SetNum(2 + EClass.rnd(3));
			eater.Talk("vomit");
			eater.Say("fairy_vomit", eater, thing);
			eater.PickOrDrop(eater.pos, thing);
		}
		food.trait.OnEat(eater);
		if (food.trait is TraitDrink)
		{
			food.trait.OnDrink(eater);
		}
	}

	public static void ProcTrait(Chara c, Card t)
	{
		foreach (Element value in t.elements.dict.Values)
		{
			if (!value.IsTrait)
			{
				continue;
			}
			if (value.Value >= 0)
			{
				switch (value.id)
				{
				case 753:
					c.CureCondition<ConPoison>(value.Value * 2);
					break;
				case 754:
					c.AddCondition<ConPeace>(value.Value * 5);
					break;
				case 755:
					c.CureCondition<ConBleed>(value.Value);
					break;
				case 756:
					c.AddCondition<ConHotspring>(value.Value * 2)?.SetPerfume();
					break;
				}
			}
			else
			{
				switch (value.id)
				{
				case 753:
					SayTaste("food_poison");
					c.AddCondition<ConPoison>(-value.Value * 10);
					break;
				case 754:
					SayTaste("food_mind");
					c.AddCondition<ConConfuse>(-value.Value * 10);
					c.AddCondition<ConInsane>(-value.Value * 10);
					c.AddCondition<ConHallucination>(-value.Value * 20);
					break;
				}
			}
		}
		void SayTaste(string _id)
		{
			if (c.IsPC)
			{
				c.Say(_id);
			}
		}
	}
}
