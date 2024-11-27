using System;

public class TraitMerchantMagic : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Magic;
		}
	}
}
