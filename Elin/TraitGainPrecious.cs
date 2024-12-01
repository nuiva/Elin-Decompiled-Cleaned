public class TraitGainPrecious : TraitItem
{
	public override bool OnUse(Chara c)
	{
		EClass.player.ModKeyItem(owner.id);
		owner.ModNum(-1);
		return true;
	}
}
