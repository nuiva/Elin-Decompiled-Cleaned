public class TraitCooker : TraitFactory
{
	public override Emo Icon => Emo.cook;

	public virtual string DestFoodID => null;

	public override string CrafterTitle => "invCook";

	public override AnimeID IdAnimeProgress => AnimeID.Jump;

	public override string idSoundProgress => "cook";
}
