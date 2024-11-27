using System;

public class StatsSleepiness : Stats
{
	public override bool TrackPhaseChange
	{
		get
		{
			return BaseStats.CC.IsPC;
		}
	}

	public const int Sleepy = 1;

	public const int VerySleepy = 2;

	public const int VeryVerySleepy = 3;
}
