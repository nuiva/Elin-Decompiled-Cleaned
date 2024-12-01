public class TraitEquipItem : TraitItem
{
	public virtual Thing EQ { get; set; }

	public override bool OnUse(Chara c)
	{
		if (EQ == owner)
		{
			EQ = null;
			Msg.Say("unequipItem", c, owner);
			EClass.pc.PlaySound("equip");
		}
		else
		{
			LayerInventory.SetDirty(EQ);
			Msg.Say("equipItem", c, owner);
			EQ = owner.Thing;
			EClass.pc.PlaySound("equip");
		}
		LayerInventory.SetDirty(owner.Thing);
		WidgetCurrentTool.dirty = true;
		return true;
	}

	public override void OnSetCardGrid(ButtonGrid b)
	{
		if (EClass.player.eqBait == owner && EClass.player.eqBait.GetRootCard() == EClass.pc)
		{
			b.Attach("equip", rightAttach: false);
		}
	}
}
