using System;

public class Zone_CaveElona : Zone
{
	public override bool UseFog
	{
		get
		{
			return base.lv <= 0;
		}
	}

	public override ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Bottom;
		}
	}
}
