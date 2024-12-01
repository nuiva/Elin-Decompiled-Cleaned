public class TraitMerchantJunk : TraitMerchant
{
	public override ShopType ShopType => ShopType.Junk;

	public override bool CanSellStolenGoods => true;

	public override string IdAmbience => "carpenter";
}
