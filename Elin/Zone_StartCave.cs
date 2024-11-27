using System;

public class Zone_StartCave : Zone
{
	public override ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Bottom;
		}
	}

	public override bool UseFog
	{
		get
		{
			return base.lv <= 0;
		}
	}
}
