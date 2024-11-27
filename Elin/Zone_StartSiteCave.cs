using System;

public class Zone_StartSiteCave : Zone_StartSite
{
	public override ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Bottom;
		}
	}

	public override string IDBaseLandFeat
	{
		get
		{
			return "bfCave,bfRuin,bfStart";
		}
	}
}
