using System;

public class TraitDoorSwing : TraitDoor
{
	public override bool IsOpen()
	{
		return this.isOpen;
	}

	public override void ToggleDoor(bool sound = true, bool refresh = true)
	{
		if (sound)
		{
			this.owner.PlaySound(this.idSound, 1f, true);
		}
		this.isOpen = !this.isOpen;
		this.count = 0;
		if (refresh)
		{
			EClass._map.RefreshSingleTile(this.owner.pos.x, this.owner.pos.z);
			EClass._map.RefreshFOV(this.owner.pos.x, this.owner.pos.z, 6, false);
		}
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		this.isOpen = false;
	}

	public bool isOpen;
}
