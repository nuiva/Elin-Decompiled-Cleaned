public class TraitEQFlower : TraitEQ
{
	public override void OnEquip(Chara c, bool onSetOwner)
	{
		c.hat = owner.sourceCard;
	}

	public override void OnUnequip(Chara c)
	{
		c.hat = null;
	}
}
