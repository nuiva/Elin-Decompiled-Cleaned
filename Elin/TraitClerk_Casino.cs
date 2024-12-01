public class TraitClerk_Casino : TraitUniqueMerchant
{
	public override int GuidePriotiy => 95;

	public override ShopType ShopType => ShopType.Casino;

	public override CurrencyType CurrencyType => CurrencyType.Casino_coin;

	public override string LangBarter => "daExchangeChip";
}
