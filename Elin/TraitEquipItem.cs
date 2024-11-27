using System;

public class TraitEquipItem : TraitItem
{
	public virtual Thing EQ { get; set; }

	public override bool OnUse(Chara c)
	{
		if (this.EQ == this.owner)
		{
			this.EQ = null;
			Msg.Say("unequipItem", c, this.owner, null, null);
			EClass.pc.PlaySound("equip", 1f, true);
		}
		else
		{
			LayerInventory.SetDirty(this.EQ);
			Msg.Say("equipItem", c, this.owner, null, null);
			this.EQ = this.owner.Thing;
			EClass.pc.PlaySound("equip", 1f, true);
		}
		LayerInventory.SetDirty(this.owner.Thing);
		WidgetCurrentTool.dirty = true;
		return true;
	}

	public override void OnSetCardGrid(ButtonGrid b)
	{
		if (EClass.player.eqBait == this.owner && EClass.player.eqBait.GetRootCard() == EClass.pc)
		{
			b.Attach("equip", false);
		}
	}
}
