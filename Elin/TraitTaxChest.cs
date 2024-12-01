public class TraitTaxChest : TraitItem
{
	public override int GuidePriotiy => 1000;

	public override bool OnUse(Chara c)
	{
		LayerDragGrid.CreateDeliver(InvOwnerDeliver.Mode.Tax, owner);
		return true;
	}
}
