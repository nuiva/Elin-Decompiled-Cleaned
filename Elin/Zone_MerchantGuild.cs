using System;

public class Zone_MerchantGuild : Zone_Civilized
{
	public override bool AllowCriminal
	{
		get
		{
			return false;
		}
	}
}
