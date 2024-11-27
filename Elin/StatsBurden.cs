using System;

public class StatsBurden : Stats
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
			return 1000;
		}
	}

	public override int GetPhase()
	{
		int num = (this.value < 100) ? 0 : ((this.value - 100) / 10 + 1);
		if (num > 9)
		{
			num = 9;
		}
		return base.source.phase[num];
	}

	public static int GetPhase(int value)
	{
		int num = (value < 100) ? 0 : ((value - 100) / 10 + 1);
		if (num > 9)
		{
			num = 9;
		}
		return EClass.pc.burden.source.phase[num];
	}

	public const int None = 0;

	public new const int Burden = 1;

	public const int BurdenHeavy = 2;

	public const int OverWeight = 3;

	public const int Squashed = 4;
}
