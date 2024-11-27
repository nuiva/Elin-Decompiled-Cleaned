using System;

public class TraitMerchantWeapon : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Weapon;
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
