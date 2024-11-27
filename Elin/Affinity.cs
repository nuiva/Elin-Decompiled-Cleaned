using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Affinity : EClass
{
	public static List<Affinity> list
	{
		get
		{
			return EClass.gamedata.affinities;
		}
	}

	public string Name
	{
		get
		{
			return Lang.GetList("affinity")[Affinity.list.IndexOf(this)];
		}
	}

	public static Affinity Get(Chara c)
	{
		Affinity.CC = c;
		foreach (Affinity affinity in Affinity.list)
		{
			if (c._affinity < affinity.value)
			{
				return affinity;
			}
		}
		return Affinity.list.LastItem<Affinity>();
	}

	public bool CanForceTradeEquip()
	{
		return Affinity.list.IndexOf(this) >= 6;
	}

	public bool CanInvite()
	{
		return EClass.debug.inviteAnytime || Affinity.list.IndexOf(this) >= 6;
	}

	public bool CanMarry()
	{
		return EClass.debug.marryAnytime || Affinity.list.IndexOf(this) >= 7;
	}

	public Thing OnGift(Thing t)
	{
		Thing result = Affinity.CC.AddThing(t.Thing, true, -1, -1);
		EClass.pc.PlaySound("build_resource", 1f, true);
		bool flag = t.HasTag(CTAG.gift);
		bool flag2 = t.category.IsChildOf(Affinity.CC.GetFavCat());
		bool flag3 = t.id == Affinity.CC.GetFavFood().id;
		if (EClass.debug.alwaysFavFood && t.trait is TraitFood)
		{
			flag3 = true;
		}
		int num = Mathf.Clamp(t.GetPrice(CurrencyType.Money, false, PriceType.Default, null) / (flag3 ? 10 : (flag2 ? 20 : 200)), 0, 50) + (flag3 ? 20 : (flag2 ? 5 : 0));
		num = num * 100 / (100 + Affinity.CC.LV * 10);
		if (flag)
		{
			num += 100;
			Affinity.CC.Say("give_ring", Affinity.CC, null, null);
			Affinity.CC.Talk("thanks3", null, null, false);
		}
		else if (flag3 || num > 20)
		{
			Affinity.CC.Talk("thanks3", null, null, false);
		}
		else if (flag2 || num > 10)
		{
			Affinity.CC.Talk("thanks", null, null, false);
		}
		else
		{
			Affinity.CC.Talk("thanks2", null, null, false);
		}
		Affinity.CC.ModAffinity(EClass.pc, num, true);
		return result;
	}

	public void OnTalkRumor()
	{
		bool flag = EClass.rnd(60 + EClass.pc.CHA * 2 + EClass.pc.Evalue(291) * 3) > 50 + this.difficulty + EClass.rnd(Affinity.CC.CHA + 1);
		Affinity.CC.ModAffinity(EClass.pc, flag ? (EClass.rnd(4) + 1) : (-EClass.rnd(4) - 1), false);
		if (!EClass.debug.unlimitedInterest)
		{
			Affinity.CC.interest -= 10 + EClass.rnd(10);
		}
		EClass.pc.ModExp(291, 20);
	}

	public int Mod(int a)
	{
		if (a < 0)
		{
			Affinity.CC._affinity += a;
			return a;
		}
		int num = 0;
		for (int i = 0; i < a; i++)
		{
			Affinity affinity = Affinity.Get(Affinity.CC);
			if (EClass.rnd(100 + affinity.difficulty) < 100)
			{
				Chara cc = Affinity.CC;
				int affinity2 = cc._affinity;
				cc._affinity = affinity2 + 1;
				num++;
			}
		}
		return num;
	}

	public static Chara CC;

	public int value;

	public int difficulty;
}
