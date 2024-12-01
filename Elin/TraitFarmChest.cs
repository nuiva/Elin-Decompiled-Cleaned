public class TraitFarmChest : TraitItem
{
	public override bool CanBeHeld => false;

	public override bool CanBeStolen => false;

	public override bool CanBeDestroyed => false;

	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsPCFaction)
		{
			owner.Destroy();
			EClass._zone.AddCard(ThingGen.CreateCurrency(50), owner.pos);
			return true;
		}
		LayerDragGrid.CreateDeliver(InvOwnerDeliver.Mode.Crop, owner);
		return true;
	}
}
