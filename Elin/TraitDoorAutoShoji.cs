using System;

public class TraitDoorAutoShoji : TraitDoorAuto
{
	public override string idSound
	{
		get
		{
			return "doorShoji";
		}
	}

	public override void ToggleDoor(bool sound = true, bool refresh = true)
	{
		if (sound)
		{
			this.owner.Say("open_shoji", null, null);
		}
		base.ToggleDoor(sound, refresh);
	}
}
