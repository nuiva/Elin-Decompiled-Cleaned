public class Zone_Underground : Zone
{
	public override bool UseFog => base.lv <= 0;

	public override bool BlockBorderExit => true;

	public override bool isClaimable => EClass.pc.homeBranch != null;

	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Down;
}
