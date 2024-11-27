using System;

public class TraitClerk_Casino : TraitUniqueMerchant
{
	public override int GuidePriotiy
	{
		get
		{
			return 95;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Casino;
		}
	}

	public override CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Casino_coin;
		}
	}

	public override string LangBarter
	{
		get
		{
			return "daExchangeChip";
		}
	}
}
