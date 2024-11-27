using System;

public class TraitMerchantDrug : TraitMerchant
{
	public override bool AllowCriminal
	{
		get
		{
			return true;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Drug;
		}
	}

	public override bool CanSellStolenGoods
	{
		get
		{
			return true;
		}
	}
}
