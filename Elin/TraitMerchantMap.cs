using System;

public class TraitMerchantMap : TraitUniqueMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Map;
		}
	}

	public override string LangBarter
	{
		get
		{
			return "daBuyMap";
		}
	}
}
