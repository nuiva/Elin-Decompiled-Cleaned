using System;

public class TraitBartender : TraitMerchantBooze
{
	public override bool CanRevive
	{
		get
		{
			return true;
		}
	}
}
