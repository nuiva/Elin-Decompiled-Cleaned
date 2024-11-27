using System;

public class TraitMerchantDye : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Dye;
		}
	}
}
