public class TraitDoorSwing : TraitDoor
{
	public bool isOpen;

	public override bool IsOpen()
	{
		return isOpen;
	}

	public override void ToggleDoor(bool sound = true, bool refresh = true)
	{
		if (sound)
		{
			owner.PlaySound(idSound);
		}
		isOpen = !isOpen;
		count = 0;
		if (refresh)
		{
			EClass._map.RefreshSingleTile(owner.pos.x, owner.pos.z);
			EClass._map.RefreshFOV(owner.pos.x, owner.pos.z);
		}
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		isOpen = false;
	}
}
