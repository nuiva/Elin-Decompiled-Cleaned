using System;

public class TraitMerchantFood : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Food;
		}
	}
}
