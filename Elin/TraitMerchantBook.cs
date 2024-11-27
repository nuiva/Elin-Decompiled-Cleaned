using System;

public class TraitMerchantBook : TraitMerchant
{
	public override int CostRerollShop
	{
		get
		{
			return 2;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			return ShopType.Book;
		}
	}
}
