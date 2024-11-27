using System;

public class TraitElder : TraitMayor
{
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
