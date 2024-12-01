public class TraitLoytel : TraitUniqueMerchant
{
	public override ShopType ShopType => ShopType.Loytel;

	public override CurrencyType CurrencyType => CurrencyType.Money2;

	public override string LangBarter => "daBuyStarter";

	public override bool CanBeBanished => false;
}
