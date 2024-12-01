public class Zone_StartSiteCave : Zone_StartSite
{
	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Bottom;

	public override string IDBaseLandFeat => "bfCave,bfRuin,bfStart";
}
