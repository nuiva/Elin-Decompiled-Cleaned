using System;

public class TraitBigSister : TraitUniqueChara
{
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

	public override bool CanInvite
	{
		get
		{
			return false;
		}
	}
}
