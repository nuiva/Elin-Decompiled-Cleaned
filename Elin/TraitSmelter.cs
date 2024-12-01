public class TraitSmelter : TraitCrafter
{
	public override string IdSource => "Smelter";

	public override ToggleType ToggleType => ToggleType.Fire;

	public override string CrafterTitle => "invSmelter";

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "craft_smelt";
}
