using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Dice
{
	public static int Roll(int num, int sides, int bonus = 0, Card card = null)
	{
		Dice.<>c__DisplayClass1_0 CS$<>8__locals1;
		CS$<>8__locals1.sides = sides;
		CS$<>8__locals1.num = num;
		CS$<>8__locals1.bonus = bonus;
		int num2 = 1;
		bool flag = true;
		int num3 = 0;
		if (card != null)
		{
			int num4 = card.Evalue(78);
			flag = (num4 >= 0);
			num2 = 1 + Mathf.Abs(num4 / 100) + ((Mathf.Abs(num4 % 100) > Dice.rnd(100)) ? 1 : 0);
		}
		for (int i = 0; i < num2; i++)
		{
			int num5 = Dice.<Roll>g__Roll|1_0(ref CS$<>8__locals1);
			if (i == 0 || (flag && num5 > num3) || (!flag && num5 < num3))
			{
				num3 = num5;
			}
		}
		return num3;
	}

	public static int RollMax(int num, int sides, int bonus = 0)
	{
		return num * sides + bonus;
	}

	public static int rnd(int a)
	{
		return Rand.Range(0, a);
	}

	public Dice(int _num = 0, int _sides = 0, int _bonus = 0, Card _card = null)
	{
		this.num = _num;
		this.sides = _sides;
		this.bonus = _bonus;
		this.card = _card;
	}

	public static Dice Parse(string raw)
	{
		Dice dice = new Dice(0, 0, 0, null);
		string[] array = raw.Split(',', StringSplitOptions.None);
		if (array.Length != 0)
		{
			string[] array2 = array[0].Split('d', StringSplitOptions.None);
			dice.num = int.Parse(array2[0]);
			dice.sides = int.Parse(array2[1]);
		}
		if (array.Length > 1)
		{
			dice.bonus = int.Parse(array[1]);
		}
		return dice;
	}

	public int Roll()
	{
		return Dice.Roll(this.num, this.sides, this.bonus, this.card);
	}

	public int RollMax()
	{
		return Dice.RollMax(this.num, this.sides, this.bonus);
	}

	public override string ToString()
	{
		return this.num.ToString() + "d" + this.sides.ToString() + ((this.bonus > 0) ? ("+" + this.bonus.ToString()) : ((this.bonus < 0) ? (this.bonus.ToString() ?? "") : ""));
	}

	public static Dice Create(Element ele, Card c)
	{
		string key = ele.source.alias;
		if (!EClass.sources.calc.map.ContainsKey(key) && !ele.source.aliasRef.IsEmpty())
		{
			key = ele.source.alias.Split('_', StringSplitOptions.None)[0] + "_";
		}
		if (!EClass.sources.calc.map.ContainsKey(key))
		{
			return null;
		}
		SourceCalc.Row row = EClass.sources.calc.map[key];
		int power = ele.GetPower(c);
		int ele2 = ele.source.aliasParent.IsEmpty() ? 0 : c.Evalue(ele.source.aliasParent);
		Dice result;
		try
		{
			result = new Dice(Mathf.Max(1, row.num.Calc(power, ele2, 0)), Mathf.Max(1, row.sides.Calc(power, ele2, 0)), row.bonus.Calc(power, ele2, 0), c);
		}
		catch
		{
			Debug.Log(ele.id);
			result = new Dice(0, 0, 0, null);
		}
		return result;
	}

	public static Dice Create(string id, int power, Card c = null, Act act = null)
	{
		if (!EClass.sources.calc.map.ContainsKey(id))
		{
			Debug.Log(id);
			return null;
		}
		SourceCalc.Row row = EClass.sources.calc.map[id];
		int power2 = power;
		int ele = power / 10;
		if (act != null)
		{
			Element orCreateElement = c.elements.GetOrCreateElement(act.source.id);
			power2 = orCreateElement.GetPower(c);
			ele = (orCreateElement.source.aliasParent.IsEmpty() ? 0 : c.Evalue(orCreateElement.source.aliasParent));
		}
		Dice result;
		try
		{
			result = new Dice(Mathf.Max(1, row.num.Calc(power2, ele, 0)), Mathf.Max(1, row.sides.Calc(power2, ele, 0)), row.bonus.Calc(power2, ele, 0), c);
		}
		catch
		{
			result = new Dice(0, 0, 0, null);
		}
		return result;
	}

	[CompilerGenerated]
	internal static int <Roll>g__Roll|1_0(ref Dice.<>c__DisplayClass1_0 A_0)
	{
		int num = 0;
		for (int i = 0; i < A_0.num; i++)
		{
			num += Dice.rnd(A_0.sides) + 1;
		}
		return num + A_0.bonus;
	}

	public static Dice Null = new Dice(0, 0, 0, null);

	public int num;

	public int sides;

	public int bonus;

	public Card card;
}
