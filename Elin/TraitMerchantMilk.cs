using System;

public class TraitMerchantMilk : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Milk;
		}
	}
}
