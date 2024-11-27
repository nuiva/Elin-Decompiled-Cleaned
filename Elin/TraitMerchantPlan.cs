using System;

public class TraitMerchantPlan : TraitMerchant
{
	public override int CostRerollShop
	{
		get
		{
			return 2;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Plan;
		}
	}

	public override CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Money2;
		}
	}
}
