using System;

public class TraitMerchantPlat : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			if (!base.owner.IsPCFaction)
			{
				return ShopType.Plat;
			}
			return ShopType.None;
		}
	}

	public override CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Plat;
		}
	}

	public override string LangBarter
	{
		get
		{
			return "daBuyPlat";
		}
	}

	public override bool CanInvite
	{
		get
		{
			return false;
		}
	}
}
