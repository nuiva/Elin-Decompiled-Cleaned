using System;

public class TraitMerchantBread : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Bread;
		}
	}
}
