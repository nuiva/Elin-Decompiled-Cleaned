using System;

public class Zone_OldGuild : Zone_Civilized
{
	public override ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Exact;
		}
	}
}
