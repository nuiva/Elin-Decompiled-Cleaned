public class TraitGrindstone : TraitCrafter
{
	public override string IdSource => "Grindstone";

	public override string CrafterTitle => "invGrind";

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "grind";

	public override string idSoundComplete => "grind_finish";

	public override int numIng => 2;

	public override bool StopSoundProgress => true;
}
