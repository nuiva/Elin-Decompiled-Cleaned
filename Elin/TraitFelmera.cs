using System;

public class TraitFelmera : TraitUniqueMerchant
{
	public override int CostRerollShop
	{
		get
		{
			return 0;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Specific;
		}
	}
}
