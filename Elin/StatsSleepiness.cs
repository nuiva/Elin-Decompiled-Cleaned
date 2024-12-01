public class StatsSleepiness : Stats
{
	public const int Sleepy = 1;

	public const int VerySleepy = 2;

	public const int VeryVerySleepy = 3;

	public override bool TrackPhaseChange => BaseStats.CC.IsPC;
}
