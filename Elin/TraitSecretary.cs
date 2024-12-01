public class TraitSecretary : TraitCitizen
{
	public override int GuidePriotiy => 99;

	public override bool CanInvestTown => true;

	public override string IDRumor => "secretary";

	public override ShopType ShopType => ShopType.Influence;

	public override CurrencyType CurrencyType => CurrencyType.Influence;

	public override string GetDramaText()
	{
		return "dramaText_town".lang((EClass._zone.development / 10).ToString() ?? "");
	}
}
