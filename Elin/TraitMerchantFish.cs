using System;

public class TraitMerchantFish : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Fish;
		}
	}
}
