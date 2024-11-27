using System;

public class TraitFiama : TraitUniqueMerchant
{
	public override bool CanInvite
	{
		get
		{
			return EClass._zone.id == "lothria";
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Starter;
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
}
