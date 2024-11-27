using System;

public class TraitMerchantLamp : TraitMerchant
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
			return ShopType.Lamp;
		}
	}
}
