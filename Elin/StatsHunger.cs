public class StatsHunger : Stats
{
	public const int Bloated = 0;

	public const int Filled = 1;

	public const int Normal = 2;

	public const int Hungry = 3;

	public const int VeryHungry = 4;

	public const int Starving = 5;

	public override bool TrackPhaseChange => BaseStats.CC.IsPC;
}
