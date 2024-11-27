using System;
using UnityEngine;

public class StatsMana : Stats
{
	public override int max
	{
		get
		{
			return Mathf.Max(1, ((BaseStats.CC.MAG * 2 + BaseStats.CC.WIL + BaseStats.CC.LER / 2) * Mathf.Min(BaseStats.CC.LV, 25) / 25 + BaseStats.CC.MAG + 10) * BaseStats.CC.Evalue(61) / 100 * (int)((BaseStats.CC.IsPCFaction ? ((Rarity)100) : ((Rarity)100 + (int)(BaseStats.CC.rarity * (Rarity)250))) + (BaseStats.CC.IsPC ? (EClass.player.lastEmptyAlly * BaseStats.CC.Evalue(1646)) : 0)) / 100);
		}
	}

	public override int min
	{
		get
		{
			return -9999;
		}
	}

	public override void Mod(int a)
	{
		if (BaseStats.CC.IsPC && EClass.debug.godMode && a < 0)
		{
			return;
		}
		if (a < 0 && BaseStats.CC.HasElement(1330, 1))
		{
			a = -EClass.rnd(-a * 130 / 100 + 2);
		}
		base.Mod(a);
		if (a < 0)
		{
			if (!BaseStats.CC.IsPCFaction)
			{
				a /= 10;
			}
			bool shouldShowMsg = BaseStats.CC.ShouldShowMsg;
			if (this.value < 0)
			{
				int num = -this.value * 400 / (100 + BaseStats.CC.Evalue(303) * 10);
				if (BaseStats.CC.HasElement(1201, 1))
				{
					num /= 3;
				}
				if (!BaseStats.CC.IsPC)
				{
					num /= 5;
					if (num < 10)
					{
						return;
					}
				}
				BaseStats.CC.Say("mana_overflow", BaseStats.CC, null, null);
				BaseStats.CC.DamageHP(num, 921, 100, AttackSource.None, null, true);
				if (BaseStats.CC.IsAliveInCurrentZone)
				{
					BaseStats.CC.elements.ModExp(303, Mathf.Clamp(-a * 10, 10, 200), false);
				}
			}
		}
	}
}
