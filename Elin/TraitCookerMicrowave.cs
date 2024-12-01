public class TraitCookerMicrowave : TraitCooker
{
	public override bool CanTriggerFire => true;

	public override ToggleType ToggleType => ToggleType.Electronics;

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "cook_micro";

	public override string idSoundComplete => "cook_micro_finish";

	public override AnimeType animeType => AnimeType.Microwave;

	public override bool AutoTurnOff => true;

	public override bool AutoToggle => false;
}
