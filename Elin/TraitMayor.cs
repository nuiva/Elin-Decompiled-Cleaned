public class TraitMayor : TraitCitizen
{
	public override int GuidePriotiy => 100;

	public override string IDRumor => "mayor";

	public override string GetDramaText()
	{
		return "dramaText_town".lang((EClass._zone.development / 10).ToString() ?? "");
	}
}
