public class Zone_LittleGarden : Zone_Civilized
{
	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Right;

	public override void OnRegenerate()
	{
		base.development = (EClass.player.little_saved * 2 - EClass.player.little_dead * 3) * 10;
	}
}
