using System;

public class TraitLoytel : TraitUniqueMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Loytel;
		}
	}

	public override CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Money2;
		}
	}

	public override string LangBarter
	{
		get
		{
			return "daBuyStarter";
		}
	}

	public override bool CanBeBanished
	{
		get
		{
			return false;
		}
	}
}
