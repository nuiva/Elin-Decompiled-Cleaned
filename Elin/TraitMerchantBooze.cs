using System;

public class TraitMerchantBooze : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Booze;
		}
	}
}
