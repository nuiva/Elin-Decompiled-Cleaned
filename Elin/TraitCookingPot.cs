public class TraitCookingPot : TraitCooker
{
	public override ToggleType ToggleType => ToggleType.Fire;

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override AnimeType animeType => AnimeType.Pot;

	public override string idSoundProgress => "cook_pot";

	public override string idSoundBG => "bg_boil";
}
