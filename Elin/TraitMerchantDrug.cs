public class TraitMerchantDrug : TraitMerchant
{
	public override bool AllowCriminal => true;

	public override ShopType ShopType => ShopType.Drug;

	public override bool CanSellStolenGoods => true;
}
