public class TraitFiama : TraitUniqueMerchant
{
	public override bool CanInvite => EClass._zone.id == "lothria";

	public override ShopType ShopType => ShopType.Starter;

	public override CurrencyType CurrencyType => CurrencyType.Money2;

	public override string LangBarter => "daBuyStarter";
}
