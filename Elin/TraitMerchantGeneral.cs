using System;

public class TraitMerchantGeneral : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.General;
		}
	}
}
