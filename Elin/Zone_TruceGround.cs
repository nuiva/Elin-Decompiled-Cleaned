using System;

public class Zone_TruceGround : Zone
{
	public override bool ShouldRegenerate
	{
		get
		{
			return global::Version.Get(base.version).IsBelow(0, 23, 7);
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
