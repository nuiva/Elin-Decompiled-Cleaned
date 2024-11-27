using System;

public class StatsStamina : Stats
{
	public override bool TrackPhaseChange
	{
		get
		{
			return BaseStats.CC.IsPC;
		}
	}

	public override int max
	{
		get
		{
			return BaseStats.CC._maxStamina * BaseStats.CC.Evalue(62) / 100;
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
		int num = BaseStats.CC.Evalue(1403);
		if (a < 0 && num > 0)
		{
			a = a * 100 / (100 + EClass.rnd(num + 1) * 20);
			if (a == 0)
			{
				return;
			}
		}
		base.Mod(a);
		if (a < 0)
		{
			bool shouldShowMsg = BaseStats.CC.ShouldShowMsg;
		}
		if (a < 0 && this.value < 0)
		{
			BaseStats.CC.DamageHP(-this.value, AttackSource.Fatigue, null);
		}
	}

	public override int GetPhase()
	{
		if (this.value < 0)
		{
			return 0;
		}
		return base.GetPhase();
	}

	public const int Exhausted = 0;

	public const int VeryTired = 1;

	public const int Tired = 2;

	public const int Fine = 3;
}
