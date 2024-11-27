using System;

public class TraitMiral : TraitUniqueMerchant
{
	public override int CostRerollShop
	{
		get
		{
			return 3;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Medal;
		}
	}

	public override CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Medal;
		}
	}

	public override string LangBarter
	{
		get
		{
			return "daBuyMedal";
		}
	}

	public override string IdAmbience
	{
		get
		{
			return "blacksmith";
		}
	}
}
