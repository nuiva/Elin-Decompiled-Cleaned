using System;

public class TraitAppraiser : TraitMerchantMagic
{
	public override int GuidePriotiy
	{
		get
		{
			return 21;
		}
	}

	public override bool CanIdentify
	{
		get
		{
			return true;
		}
	}
}
