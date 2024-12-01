public class TraitMerchantPlan : TraitMerchant
{
	public override int CostRerollShop => 2;

	public override ShopType ShopType => ShopType.Plan;

	public override CurrencyType CurrencyType => CurrencyType.Money2;
}
