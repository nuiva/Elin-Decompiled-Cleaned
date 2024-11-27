using System;

public class TraitNino : TraitUniqueMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.StarterEx;
		}
	}

	public override CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Money2;
		}
	}

	public override bool CanGuide
	{
		get
		{
			return true;
		}
	}

	public override string GetDramaText()
	{
		return "dramaText_town".lang((EClass._zone.development / 10).ToString() ?? "", null, null, null, null);
	}
}
