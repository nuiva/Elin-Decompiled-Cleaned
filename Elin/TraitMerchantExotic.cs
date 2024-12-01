public class TraitMerchantExotic : TraitMerchant
{
	public override int CostRerollShop => 2;

	public override ShopType ShopType
	{
		get
		{
			if (!EClass.player.IsMerchantGuildMember || Guild.Merchant.relation.rank < 4)
			{
				return ShopType.None;
			}
			return ShopType.Exotic;
		}
	}
}
