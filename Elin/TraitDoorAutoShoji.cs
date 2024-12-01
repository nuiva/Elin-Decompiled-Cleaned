public class TraitDoorAutoShoji : TraitDoorAuto
{
	public override string idSound => "doorShoji";

	public override void ToggleDoor(bool sound = true, bool refresh = true)
	{
		if (sound)
		{
			owner.Say("open_shoji");
		}
		base.ToggleDoor(sound, refresh);
	}
}
