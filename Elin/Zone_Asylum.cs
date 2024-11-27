using System;

public class Zone_Asylum : Zone_Civilized
{
	public override bool UseFog
	{
		get
		{
			return base.lv <= 0;
		}
	}
}
