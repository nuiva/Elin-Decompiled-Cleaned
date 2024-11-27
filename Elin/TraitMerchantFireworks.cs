using System;

public class TraitMerchantFireworks : TraitMerchant
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
			return ShopType.Fireworks;
		}
	}
}
