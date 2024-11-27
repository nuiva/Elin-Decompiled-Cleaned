using System;

public class TraitFarris : TraitUniqueMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Farris;
		}
	}

	public override bool CanBeBanished
	{
		get
		{
			return false;
		}
	}
}
