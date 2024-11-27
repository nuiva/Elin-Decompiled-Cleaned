using System;

public class TraitMerchantDeed : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Deed;
		}
	}

	public override int CostRerollShop
	{
		get
		{
			if (!EClass.debug.enable)
			{
				return 0;
			}
			return 1;
		}
	}
}
