public class TraitGemCutter : TraitCrafter
{
	public override string IdSource => "GemCutter";

	public override string CrafterTitle => "invGemCutter";

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "craft_saw";

	public override int MaxFuel => 100;

	public override ToggleType ToggleType => ToggleType.Electronics;
}
