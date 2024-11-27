using System;

public class TraitMerchantFruit : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Fruit;
		}
	}
}
