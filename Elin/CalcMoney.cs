using System;
using UnityEngine;

public class CalcMoney : EClass
{
	public static int Negotiate(int a, float mod = 1.5f)
	{
		return (int)Mathf.Max((float)((long)a * 100L / (long)(100f + (float)Mathf.Max(0, EClass.pc.CHA / 2 + EClass.pc.Evalue(291)) * mod)), 1f);
	}

	public static int Invest(int a, float mod = 2f)
	{
		return (int)Mathf.Max((float)((long)a * 100L / (long)(100f + (float)Mathf.Max(0, EClass.pc.CHA / 2 + EClass.pc.Evalue(292)) * mod)), 1f);
	}

	public static int Meal(Chara c)
	{
		return CalcMoney.Negotiate(Guild.Fighter.ServicePrice(70), 1.5f);
	}

	public static int Heal(Chara c)
	{
		return CalcMoney.Negotiate(Guild.Fighter.ServicePrice(100), 1.5f);
	}

	public static int Picklock(Chara c, Thing t)
	{
		return CalcMoney.Negotiate(Guild.Fighter.ServicePrice(t.c_lockLv * 65 + 75), 1.5f);
	}

	public static int Identify(Chara c, bool superior)
	{
		return CalcMoney.Negotiate(Guild.Fighter.ServicePrice(superior ? 750 : 50), 1.5f);
	}

	public static int Revive(Chara c)
	{
		return CalcMoney.Negotiate((c.LV + 5) * (c.LV + 5) * 3, 1.5f);
	}

	public static int BuySlave(Chara c)
	{
		return CalcMoney.Negotiate((c.LV + 5) * (c.LV + 5) * 20 + Rand.rndSeed(c.LV * 20, c.uid), 1.5f);
	}

	public static int SellSlave(Chara c)
	{
		return (c.LV + 5) * (c.LV + 5) * 5;
	}

	public static int Whore(Chara c)
	{
		return CalcMoney.Negotiate(c.CHA * 4 + 20 + EClass.player.tempFame / 10, 1.5f);
	}

	public static int InvestShop(Chara c, Chara tc)
	{
		return CalcMoney.Invest(Guild.Merchant.InvestPrice(Mathf.Max(tc.c_invest * 700, tc.c_invest * tc.c_invest * 80) + 200), 2f);
	}

	public static int InvestZone(Chara c)
	{
		return CalcMoney.Invest((int)Mathf.Max((float)((long)EClass._zone.development * 50L), (float)(EClass._zone.development * EClass._zone.development / 4)) + 500, 2f);
	}
}
