using System;

public class TraitNola : TraitUniqueMerchant
{
	public override int GuidePriotiy
	{
		get
		{
			return 99;
		}
	}

	public override bool CanInvestTown
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

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Influence;
		}
	}

	public override CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Influence;
		}
	}
}
