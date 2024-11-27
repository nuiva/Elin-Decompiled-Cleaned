using System;

public class TraitMerchantFestival : TraitMerchant
{
	public override int CostRerollShop
	{
		get
		{
			return 5;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Festival;
		}
	}
}
