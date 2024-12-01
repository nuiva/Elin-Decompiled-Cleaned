public class TraitSeeker : TraitUniqueMerchant
{
	public override ShopType ShopType => ShopType.RedBook;

	public override string LangBarter => "daBorrowBook";

	public override CurrencyType CurrencyType => CurrencyType.None;

	public override int RestockDay => 360;
}
