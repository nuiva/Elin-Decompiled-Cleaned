public class StatsBurden : Stats
{
	public const int None = 0;

	public new const int Burden = 1;

	public const int BurdenHeavy = 2;

	public const int OverWeight = 3;

	public const int Squashed = 4;

	public override bool TrackPhaseChange => BaseStats.CC.IsPC;

	public override int max => 1000;

	public override int GetPhase()
	{
		int num = ((value >= 100) ? ((value - 100) / 10 + 1) : 0);
		if (num > 9)
		{
			num = 9;
		}
		return base.source.phase[num];
	}

	public static int GetPhase(int value)
	{
		int num = ((value >= 100) ? ((value - 100) / 10 + 1) : 0);
		if (num > 9)
		{
			num = 9;
		}
		return EClass.pc.burden.source.phase[num];
	}
}
