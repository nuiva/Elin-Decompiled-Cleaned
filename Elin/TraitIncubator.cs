public class TraitIncubator : TraitCrafter
{
	public override string IdSource => "Incubator";

	public override string CrafterTitle => "invIncubator";

	public override ToggleType ToggleType => ToggleType.Electronics;

	public override string idSoundProgress => "cook_micro";

	public override string idSoundComplete => "egg";

	public override AnimeType animeType => AnimeType.Microwave;

	public override bool AutoTurnOff => true;

	public override bool AutoToggle => false;

	public override bool CanUseFromInventory => false;
}
