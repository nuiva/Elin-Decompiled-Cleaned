public class TraitBigSister : TraitUniqueChara
{
	public override ShopType ShopType => ShopType.Influence;

	public override CurrencyType CurrencyType => CurrencyType.Influence;

	public override bool CanInvite => false;

	public override string GetDramaText()
	{
		return "dramaText_town".lang((EClass._zone.development / 10).ToString() ?? "");
	}
}
