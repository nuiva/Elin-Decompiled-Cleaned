using System;

public class Zone_Underground : Zone
{
	public override bool UseFog
	{
		get
		{
			return base.lv <= 0;
		}
	}

	public override bool BlockBorderExit
	{
		get
		{
			return true;
		}
	}

	public override bool isClaimable
	{
		get
		{
			return EClass.pc.homeBranch != null;
		}
	}

	public override ZoneTransition.EnterState RegionEnterState
	{
		get
		{
			return ZoneTransition.EnterState.Down;
		}
	}
}
