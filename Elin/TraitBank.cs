public class TraitBank : TraitItem
{
	public override ThrowType ThrowType => ThrowType.Vase;

	public override bool OnUse(Chara c)
	{
		LayerDragGrid.CreateDeliver(InvOwnerDeliver.Mode.Bank, owner);
		return true;
	}
}
