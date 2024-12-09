using UnityEngine;

public class CalcMoney : EClass
{
	public static int Negotiate(int a, float mod = 1.5f)
	{
		return (int)Mathf.Max((long)a * 100L / (long)(100f + (float)Mathf.Max(0, EClass.pc.CHA / 2 + EClass.pc.Evalue(291 /* negotiation */)) * mod), 1f);
	}

	public static int Invest(int a, float mod = 2f)
	{
		return (int)Mathf.Max((long)a * 100L / (long)(100f + (float)Mathf.Max(0, EClass.pc.CHA / 2 + EClass.pc.Evalue(292 /* investing */)) * mod), 1f);
	}

	public static int Meal(Chara c)
	{
		return Negotiate(Guild.Fighter.ServicePrice(70));
	}

	public static int Heal(Chara c)
	{
		return Negotiate(Guild.Fighter.ServicePrice(100));
	}

	public static int Picklock(Chara c, Thing t)
	{
		return Negotiate(Guild.Fighter.ServicePrice(t.c_lockLv * 65 + 75));
	}

	public static int Identify(Chara c, bool superior)
	{
		return Negotiate(Guild.Fighter.ServicePrice(superior ? 750 : 50));
	}

	public static int Revive(Chara c)
	{
		return Negotiate((c.LV + 5) * (c.LV + 5) * 3);
	}

	public static int BuySlave(Chara c)
	{
		return Negotiate((c.LV + 5) * (c.LV + 5) * 20 + Rand.rndSeed(c.LV * 20, c.uid));
	}

	public static int SellSlave(Chara c)
	{
		return (c.LV + 5) * (c.LV + 5) * 5;
	}

	public static int Whore(Chara c)
	{
		return Negotiate(c.CHA * 4 + 20 + EClass.player.tempFame / 10);
	}

	public static int InvestShop(Chara c, Chara tc)
	{
		return Invest(Guild.Merchant.InvestPrice(Mathf.Max(tc.c_invest * 700, tc.c_invest * tc.c_invest * 80) + 200));
	}

	public static int InvestZone(Chara c)
	{
		return Invest((int)Mathf.Max((long)EClass._zone.development * 50L, EClass._zone.development * EClass._zone.development / 4) + 500);
	}
}
