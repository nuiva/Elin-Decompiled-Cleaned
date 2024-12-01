public class TraitKiln : TraitCrafter
{
	public override string IdSource => "Kiln";

	public override ToggleType ToggleType => ToggleType.Fire;

	public override string CrafterTitle => "invKiln";

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "craft_smelt";
}
