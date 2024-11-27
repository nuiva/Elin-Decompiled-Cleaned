using System;

public class TraitMerchantFurniture : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Furniture;
		}
	}

	public override string IdAmbience
	{
		get
		{
			return "carpenter";
		}
	}
}
