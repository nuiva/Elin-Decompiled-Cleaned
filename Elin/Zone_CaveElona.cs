public class Zone_CaveElona : Zone
{
	public override bool UseFog => base.lv <= 0;

	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Bottom;
}
