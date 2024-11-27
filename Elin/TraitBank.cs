using System;

public class TraitBank : TraitItem
{
	public override ThrowType ThrowType
	{
		get
		{
			return ThrowType.Vase;
		}
	}

	public override bool OnUse(Chara c)
	{
		LayerDragGrid.CreateDeliver(InvOwnerDeliver.Mode.Bank, this.owner);
		return true;
	}
}
