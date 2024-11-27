using System;

public class Zone_Lothria : Zone_Town
{
	public override ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Right;
		}
	}
}
