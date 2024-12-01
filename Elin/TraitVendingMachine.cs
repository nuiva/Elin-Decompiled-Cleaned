public class TraitVendingMachine : TraitItem
{
	public override string IDInvStyle => "modern";

	public override bool ShowOrbit => true;

	public override ShopType ShopType => ShopType.VMachine;

	public override bool AllowSell => false;

	public override bool OnUse(Chara c)
	{
		OnBarter();
		EClass.ui.AddLayer(LayerInventory.CreateBuy(owner, c.trait.CurrencyType, c.trait.PriceType));
		return false;
	}
}
