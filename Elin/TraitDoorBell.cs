using System;

public class TraitDoorBell : Trait
{
	public override void OnOpenDoor(Chara c)
	{
		this.owner.PlaySound(base.GetParam(1, null), 1f, true);
	}
}
