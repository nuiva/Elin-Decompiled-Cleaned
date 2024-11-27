using System;

public class TraitSecretary : TraitCitizen
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

	public override string IDRumor
	{
		get
		{
			return "secretary";
		}
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
