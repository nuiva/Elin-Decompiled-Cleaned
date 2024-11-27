using System;

public class TraitLicense : TraitScroll
{
	public override void OnRead(Chara c)
	{
		EClass.player.ModKeyItem(this.owner.id, 1, true);
		this.owner.ModNum(-1, true);
	}
}
