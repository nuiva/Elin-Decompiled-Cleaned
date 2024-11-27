using System;

public class Zone_EmbassyPalmia : Zone_Civilized
{
	public override ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Bottom;
		}
	}

	public override bool AllowCriminal
	{
		get
		{
			return false;
		}
	}
}
