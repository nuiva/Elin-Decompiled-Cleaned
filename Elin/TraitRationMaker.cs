public class TraitRationMaker : TraitCrafter
{
	public override string IdSource => "RationMaker";

	public override int numIng => 2;

	public override string CrafterTitle => "invMill";

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "cook_grind";
}
