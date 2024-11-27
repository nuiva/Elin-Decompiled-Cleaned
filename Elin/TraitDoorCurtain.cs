using System;

public class TraitDoorCurtain : TraitDoor
{
	public override bool IsOpen()
	{
		return true;
	}

	public override void ToggleDoor(bool sound = false, bool refresh = true)
	{
	}

	public override void TrySetAct(ActPlan p)
	{
	}
}
