using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FoodEffect : EClass
{
	public static bool IsHumanFlesh(Thing food)
	{
		return !food.HasTag(CTAG.notHumanMeat) && (FoodEffect.IsHumanFlesh(food.refCard) || FoodEffect.IsHumanFlesh(food.refCard2));
	}

	public static bool IsUndeadFlesh(Thing food)
	{
		return FoodEffect.IsUndeadFlesh(food.refCard) || FoodEffect.IsUndeadFlesh(food.refCard2);
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
		return r.isChara && EClass.sources.races.map[EClass.sources.charas.map[r.id].race].tag.Contains("human");
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
		return r.isChara && EClass.sources.races.map[EClass.sources.charas.map[r.id].race].tag.Contains("undead");
	}

	public static void Proc(Chara c, Thing food)
	{
		FoodEffect.<>c__DisplayClass4_0 CS$<>8__locals1;
		CS$<>8__locals1.c = c;
		bool flag = EClass._zone.IsPCFaction && CS$<>8__locals1.c.IsInSpot<TraitSpotDining>();
		int num = food.isCrafted ? ((EClass.pc.Evalue(1650) >= 3) ? 5 : 0) : 0;
		float num2 = (float)(100 + (flag ? 10 : 0) + num + Mathf.Min(food.QualityLv * 10, 100)) / 200f;
		if (num2 < 0.1f)
		{
			num2 = 0.1f;
		}
		int num3 = food.Evalue(10);
		float num4 = 40f;
		float num5 = 1f;
		CS$<>8__locals1.idTaste = "";
		bool flag2 = FoodEffect.IsHumanFlesh(food);
		bool flag3 = FoodEffect.IsUndeadFlesh(food);
		if (food.source._origin != "meat" && food.source._origin != "dish")
		{
			flag3 = (flag2 = false);
		}
		if (food.id == "deadbody")
		{
			flag2 = true;
		}
		string[] components = food.source.components;
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i].Contains("egg"))
			{
				flag2 = false;
			}
		}
		bool flag4 = CS$<>8__locals1.c.HasElement(1205, 1);
		bool flag5 = food.IsDecayed || flag3;
		if (food.IsBlessed)
		{
			num2 *= 1.5f;
		}
		if (food.IsCursed)
		{
			num2 *= 0.5f;
		}
		if (flag4)
		{
			if (flag2)
			{
				num5 *= 2f;
				num2 *= 1.5f;
			}
			else
			{
				num5 *= 0.5f;
				num2 /= 2f;
				num3 /= 2;
			}
		}
		else if (flag2)
		{
			num5 = 0f;
			num2 *= 0.5f;
		}
		if (CS$<>8__locals1.c.HasElement(1200, 1))
		{
			num2 *= 1.25f;
		}
		if (!CS$<>8__locals1.c.IsPC)
		{
			num2 *= 3f;
		}
		if (flag5 && !CS$<>8__locals1.c.HasElement(480, 1))
		{
			if (CS$<>8__locals1.c.IsPC)
			{
				if (flag3)
				{
					CS$<>8__locals1.c.Say("food_undead", null, null);
				}
				CS$<>8__locals1.c.Say("food_rot", null, null);
			}
			num5 = 0f;
			num3 /= 2;
		}
		else
		{
			string text = food.source._origin;
			if (!(text == "meat"))
			{
				if (!(text == "fish"))
				{
					if (text == "dough")
					{
						if (CS$<>8__locals1.c.IsPC)
						{
							CS$<>8__locals1.c.Say("food_raw_powder", null, null);
						}
						num2 *= 0.9f;
						num5 = 0.5f;
					}
				}
				else if (CS$<>8__locals1.c.IsHuman)
				{
					if (CS$<>8__locals1.c.IsPC)
					{
						CS$<>8__locals1.c.Say("food_raw_fish", null, null);
					}
					num2 *= 0.9f;
					num5 = 0.5f;
				}
			}
			else
			{
				if (CS$<>8__locals1.c.IsPC)
				{
					CS$<>8__locals1.c.Say("food_raw_meat", null, null);
				}
				num2 *= 0.7f;
				num5 = 0.5f;
			}
		}
		float num6 = (float)Mathf.Min(CS$<>8__locals1.c.hunger.value, num3);
		if (CS$<>8__locals1.c.hunger.GetPhase() >= 3)
		{
			num6 *= 1.1f;
		}
		if (flag5 && !CS$<>8__locals1.c.HasElement(480, 1))
		{
			CS$<>8__locals1.c.ModExp(70, -300);
			CS$<>8__locals1.c.ModExp(71, -300);
			CS$<>8__locals1.c.ModExp(72, -200);
			CS$<>8__locals1.c.ModExp(73, -200);
			CS$<>8__locals1.c.ModExp(74, -200);
			CS$<>8__locals1.c.ModExp(75, 500);
			CS$<>8__locals1.c.ModExp(76, -200);
			CS$<>8__locals1.c.ModExp(77, -300);
		}
		else
		{
			num2 = num2 * num6 / 10f;
			if (CS$<>8__locals1.c.HasCondition<ConAnorexia>())
			{
				num2 = 0.01f;
			}
			List<Element> list = food.ListValidTraits(true, false);
			foreach (Element element in food.elements.dict.Values)
			{
				if (!element.source.foodEffect.IsEmpty() && list.Contains(element))
				{
					string[] foodEffect = element.source.foodEffect;
					int num7 = element.id;
					float num8 = num2 * (float)element.Value;
					if (element.source.category == "food" && CS$<>8__locals1.c.IsPC)
					{
						bool flag6 = num8 >= 0f;
						string text2 = element.source.GetText(flag6 ? "textInc" : "textDec", true);
						if (text2 != null)
						{
							Msg.SetColor(flag6 ? "positive" : "negative");
							CS$<>8__locals1.c.Say(text2, null, null);
						}
					}
					string text = foodEffect[0];
					uint num9 = <PrivateImplementationDetails>.ComputeStringHash(text);
					if (num9 <= 1923516200U)
					{
						if (num9 <= 893270296U)
						{
							if (num9 != 207713729U)
							{
								if (num9 == 893270296U)
								{
									if (text == "loseWeight")
									{
										CS$<>8__locals1.c.ModWeight(-EClass.rndHalf(element.Value), true);
									}
								}
							}
							else if (text == "little")
							{
								int @int = CS$<>8__locals1.c.GetInt(112, null);
								if (@int < 30)
								{
									CS$<>8__locals1.c.Say("little_eat", CS$<>8__locals1.c, null, null);
									CS$<>8__locals1.c.PlaySound("ding_potential", 1f, true);
									int v = Mathf.Max(5 - @int / 2, 1);
									Debug.Log("sister eaten:" + @int.ToString() + "/" + v.ToString());
									foreach (Element element2 in CS$<>8__locals1.c.elements.dict.Values)
									{
										if (element2.IsMainAttribute)
										{
											CS$<>8__locals1.c.elements.ModPotential(element2.id, v);
										}
									}
									if (CS$<>8__locals1.c.race.id == "mutant" && CS$<>8__locals1.c.elements.Base(1230) < 10)
									{
										CS$<>8__locals1.c.Say("little_adam", CS$<>8__locals1.c, null, null);
										CS$<>8__locals1.c.SetFeat(1230, CS$<>8__locals1.c.elements.Base(1230) + 1, false);
									}
									CS$<>8__locals1.c.SetInt(112, @int + 1);
								}
							}
						}
						else if (num9 != 1429431836U)
						{
							if (num9 == 1923516200U)
							{
								if (text == "exp")
								{
									num7 = ((foodEffect.Length > 1) ? EClass.sources.elements.alias[foodEffect[1]].id : element.id);
									int a = (int)(num8 * (float)((foodEffect.Length > 2) ? foodEffect[2].ToInt() : 4)) * 2 / 3;
									CS$<>8__locals1.c.ModExp(num7, a);
								}
							}
						}
						else if (text == "pot")
						{
							num7 = ((foodEffect.Length > 1) ? EClass.sources.elements.alias[foodEffect[1]].id : element.id);
							int vTempPotential = CS$<>8__locals1.c.elements.GetElement(num7).vTempPotential;
							int num10 = EClass.rndHalf((int)(num8 / 5f) + 1);
							num10 = num10 * 100 / Mathf.Max(100, vTempPotential * 2 / 3);
							CS$<>8__locals1.c.elements.ModTempPotential(num7, num10, 8);
						}
					}
					else if (num9 <= 2676017222U)
					{
						if (num9 != 2399918791U)
						{
							if (num9 == 2676017222U)
							{
								if (text == "gainWeight")
								{
									CS$<>8__locals1.c.ModWeight(EClass.rndHalf(element.Value), true);
								}
							}
						}
						else if (text == "karma")
						{
							if (CS$<>8__locals1.c.IsPCParty)
							{
								EClass.player.ModKarma(-5);
							}
						}
					}
					else if (num9 != 3470992305U)
					{
						if (num9 == 3576140497U)
						{
							if (text == "love")
							{
								ActEffect.LoveMiracle(CS$<>8__locals1.c, EClass.pc, element.Value * 10);
							}
						}
					}
					else if (text == "poison")
					{
						ActEffect.Poison(CS$<>8__locals1.c, EClass.pc, element.Value * 10);
						if (CS$<>8__locals1.c.isDead)
						{
							return;
						}
					}
				}
			}
		}
		FoodEffect.ProcTrait(CS$<>8__locals1.c, food);
		num4 += (float)food.Evalue(70);
		num4 += (float)(food.Evalue(72) / 2);
		num4 += (float)(food.Evalue(73) / 2);
		num4 += (float)(food.Evalue(75) / 2);
		num4 += (float)(food.Evalue(76) * 3 / 2);
		num4 += (float)food.Evalue(440);
		num4 += (float)(food.Evalue(445) / 2);
		num4 -= (float)food.Evalue(71);
		num4 -= (float)(num3 / 2);
		num4 *= num5;
		if (CS$<>8__locals1.idTaste.IsEmpty())
		{
			if (num4 > 100f)
			{
				CS$<>8__locals1.idTaste = "food_great";
			}
			else if (num4 > 70f)
			{
				CS$<>8__locals1.idTaste = "food_good";
			}
			else if (num4 > 50f)
			{
				CS$<>8__locals1.idTaste = "food_soso";
			}
			else if (num4 > 30f)
			{
				CS$<>8__locals1.idTaste = "food_average";
			}
			else
			{
				CS$<>8__locals1.idTaste = "food_bad";
			}
			if (CS$<>8__locals1.c.IsPC)
			{
				CS$<>8__locals1.c.Say(CS$<>8__locals1.idTaste, null, null);
				if (flag2)
				{
					CS$<>8__locals1.c.Say(flag4 ? "food_human_pos" : "food_human_neg", null, null);
				}
				else if (flag4)
				{
					CS$<>8__locals1.c.Say("food_human_whine", null, null);
				}
			}
		}
		if (LangGame.Has(CS$<>8__locals1.idTaste + "2"))
		{
			CS$<>8__locals1.c.Say(CS$<>8__locals1.idTaste + "2", CS$<>8__locals1.c, food, null, null);
		}
		if (!CS$<>8__locals1.c.IsPCParty)
		{
			num3 *= 2;
		}
		num3 = num3 * (100 + CS$<>8__locals1.c.Evalue(1235) * 10) / (100 + CS$<>8__locals1.c.Evalue(1234) * 10);
		CS$<>8__locals1.c.hunger.Mod(-num3);
		if (flag2)
		{
			if (!flag4)
			{
				if (CS$<>8__locals1.c.IsHuman)
				{
					CS$<>8__locals1.c.AddCondition<ConInsane>(200, false);
					CS$<>8__locals1.c.SAN.Mod(15);
				}
				if (EClass.rnd(CS$<>8__locals1.c.IsHuman ? 5 : 20) == 0)
				{
					CS$<>8__locals1.c.SetFeat(1205, 1, true);
					flag4 = true;
				}
			}
			if (flag4)
			{
				CS$<>8__locals1.c.SetInt(31, EClass.world.date.GetRaw(0) + 10080);
			}
		}
		else if (flag4 && CS$<>8__locals1.c.GetInt(31, null) < EClass.world.date.GetRaw(0))
		{
			CS$<>8__locals1.c.SetFeat(1205, 0, true);
		}
		if (flag5 && !CS$<>8__locals1.c.HasElement(480, 1))
		{
			CS$<>8__locals1.c.AddCondition<ConParalyze>(100, false);
			CS$<>8__locals1.c.AddCondition<ConConfuse>(200, false);
		}
		if (CS$<>8__locals1.c.HasCondition<ConAnorexia>())
		{
			CS$<>8__locals1.c.Vomit();
		}
		if (num3 > 20 && CS$<>8__locals1.c.HasElement(1413, 1))
		{
			Thing thing = ThingGen.Create("seed", -1, -1);
			if (EClass.rnd(EClass.debug.enable ? 1 : 10) == 0)
			{
				TraitSeed.ApplySeed(thing, 90);
			}
			thing.SetNum(2 + EClass.rnd(3));
			CS$<>8__locals1.c.Talk("vomit", null, null, false);
			CS$<>8__locals1.c.Say("fairy_vomit", CS$<>8__locals1.c, thing, null, null);
			CS$<>8__locals1.c.PickOrDrop(CS$<>8__locals1.c.pos, thing, true);
		}
		food.trait.OnEat(CS$<>8__locals1.c);
		if (food.trait is TraitDrink)
		{
			food.trait.OnDrink(CS$<>8__locals1.c);
		}
	}

	public static void ProcTrait(Chara c, Card t)
	{
		FoodEffect.<>c__DisplayClass5_0 CS$<>8__locals1;
		CS$<>8__locals1.c = c;
		foreach (Element element in t.elements.dict.Values)
		{
			if (element.IsTrait)
			{
				if (element.Value >= 0)
				{
					switch (element.id)
					{
					case 753:
						CS$<>8__locals1.c.CureCondition<ConPoison>(element.Value * 2);
						break;
					case 754:
						CS$<>8__locals1.c.AddCondition<ConPeace>(element.Value * 5, false);
						break;
					case 755:
						CS$<>8__locals1.c.CureCondition<ConBleed>(element.Value);
						break;
					}
				}
				else
				{
					int id = element.id;
					if (id != 753)
					{
						if (id == 754)
						{
							FoodEffect.<ProcTrait>g__SayTaste|5_0("food_mind", ref CS$<>8__locals1);
							CS$<>8__locals1.c.AddCondition<ConConfuse>(-element.Value * 10, false);
							CS$<>8__locals1.c.AddCondition<ConInsane>(-element.Value * 10, false);
							CS$<>8__locals1.c.AddCondition<ConHallucination>(-element.Value * 20, false);
						}
					}
					else
					{
						FoodEffect.<ProcTrait>g__SayTaste|5_0("food_poison", ref CS$<>8__locals1);
						CS$<>8__locals1.c.AddCondition<ConPoison>(-element.Value * 10, false);
					}
				}
			}
		}
	}

	[CompilerGenerated]
	internal static void <Proc>g__SayTaste|4_0(string _id, ref FoodEffect.<>c__DisplayClass4_0 A_1)
	{
		A_1.idTaste = _id;
		if (A_1.c.IsPC)
		{
			A_1.c.Say(A_1.idTaste, null, null);
		}
	}

	[CompilerGenerated]
	internal static void <ProcTrait>g__SayTaste|5_0(string _id, ref FoodEffect.<>c__DisplayClass5_0 A_1)
	{
		if (A_1.c.IsPC)
		{
			A_1.c.Say(_id, null, null);
		}
	}
}
