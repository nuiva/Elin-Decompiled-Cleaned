using System;

public class TraitHealer : TraitCitizen
{
	public override int GuidePriotiy
	{
		get
		{
			return 20;
		}
	}

	public override bool CanHeal
	{
		get
		{
			return true;
		}
	}

	public override bool CanInvest
	{
		get
		{
			return true;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Healer;
		}
	}
}
