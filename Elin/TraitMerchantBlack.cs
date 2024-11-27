using System;

public class TraitMerchantBlack : TraitMerchant
{
	public override bool AllowCriminal
	{
		get
		{
			return true;
		}
	}

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
			if (!Guild.Thief.IsCurrentZone || Guild.Thief.relation.rank >= 4)
			{
				return ShopType.Blackmarket;
			}
			return ShopType.None;
		}
	}

	public override bool CanSellStolenGoods
	{
		get
		{
			return true;
		}
	}
}
