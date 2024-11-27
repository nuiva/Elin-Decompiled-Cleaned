using System;

public class Zone_StartSite : Zone
{
	public override bool UseFog
	{
		get
		{
			return base.lv < 0;
		}
	}

	public override bool isClaimable
	{
		get
		{
			return true;
		}
	}

	public override string IDBaseLandFeat
	{
		get
		{
			return "bfPlain,bfFertile,bfStart";
		}
	}
}
