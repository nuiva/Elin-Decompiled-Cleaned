public class TraitNino : TraitUniqueMerchant
{
	public override ShopType ShopType => ShopType.StarterEx;

	public override CurrencyType CurrencyType => CurrencyType.Money2;

	public override bool CanGuide => true;

	public override string GetDramaText()
	{
		return "dramaText_town".lang((EClass._zone.development / 10).ToString() ?? "");
	}
}
