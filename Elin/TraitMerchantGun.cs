using System;

public class TraitMerchantGun : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Gun;
		}
	}

	public override string IdAmbience
	{
		get
		{
			return "blacksmith";
		}
	}
}
