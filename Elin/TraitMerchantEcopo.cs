using System;

public class TraitMerchantEcopo : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Ecopo;
		}
	}

	public override CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Ecopo;
		}
	}
}
