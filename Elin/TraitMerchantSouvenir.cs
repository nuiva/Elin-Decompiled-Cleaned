using System;

public class TraitMerchantSouvenir : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Souvenir;
		}
	}
}
