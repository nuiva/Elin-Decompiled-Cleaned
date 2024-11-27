using System;

public class TraitSeesaw : TraitFloorSwitch
{
	public override bool UseAltTiles
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public override void OnActivateTrap(Chara c)
	{
		this.owner.isOn = !this.owner.isOn;
		this.owner.PlaySound("seesaw", 1f, true);
		TraitSwitch.haltMove = false;
	}
}
