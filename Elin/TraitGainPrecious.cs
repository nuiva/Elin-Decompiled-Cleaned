using System;

public class TraitGainPrecious : TraitItem
{
	public override bool OnUse(Chara c)
	{
		EClass.player.ModKeyItem(this.owner.id, 1, true);
		this.owner.ModNum(-1, true);
		return true;
	}
}
