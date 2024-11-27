using System;

public class TraitMerchantMeat : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Meat;
		}
	}
}
