public class TraitMerchantBook : TraitMerchant
{
	public override int CostRerollShop => 2;

	public override ShopType ShopType => ShopType.Book;
}
