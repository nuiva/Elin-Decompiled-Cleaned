public class TraitButcher : TraitCrafter
{
	public override string IdSource => "Butcher";

	public override bool CanUseFromInventory => true;

	public override string CrafterTitle => "invButcher";

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "cook_cut";
}
