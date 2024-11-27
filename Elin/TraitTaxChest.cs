using System;

public class TraitTaxChest : TraitItem
{
	public override int GuidePriotiy
	{
		get
		{
			return 1000;
		}
	}

	public override bool OnUse(Chara c)
	{
		LayerDragGrid.CreateDeliver(InvOwnerDeliver.Mode.Tax, this.owner);
		return true;
	}
}
