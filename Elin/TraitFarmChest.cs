using System;

public class TraitFarmChest : TraitItem
{
	public override bool CanBeHeld
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeStolen
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsPCFaction)
		{
			this.owner.Destroy();
			EClass._zone.AddCard(ThingGen.CreateCurrency(50, "money"), this.owner.pos);
			return true;
		}
		LayerDragGrid.CreateDeliver(InvOwnerDeliver.Mode.Crop, this.owner);
		return true;
	}
}
