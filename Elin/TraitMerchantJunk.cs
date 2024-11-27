using System;

public class TraitMerchantJunk : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			return ShopType.Junk;
		}
	}

	public override bool CanSellStolenGoods
	{
		get
		{
			return true;
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
