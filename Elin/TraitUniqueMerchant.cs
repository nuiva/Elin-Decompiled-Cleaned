using System;

public class TraitUniqueMerchant : TraitUniqueChara
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.General;
		}
	}
}
