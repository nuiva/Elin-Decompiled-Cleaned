public class Zone_StartCave : Zone
{
	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Bottom;

	public override bool UseFog => base.lv <= 0;
}
