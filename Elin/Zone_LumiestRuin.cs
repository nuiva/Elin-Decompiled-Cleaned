using System;

public class Zone_LumiestRuin : Zone
{
	public override bool UseFog
	{
		get
		{
			return base.lv <= 0;
		}
	}

	public override void OnActivate()
	{
		int visitCount = base.visitCount;
	}
}
