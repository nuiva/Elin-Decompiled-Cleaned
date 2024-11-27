using System;

public class QuestCrafter : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		switch (this.phase)
		{
		case 0:
			return EClass._map.Installed.Find("workbench", -1, -1, false) != null;
		case 1:
		{
			Thing equippedThing = EClass.pc.body.GetEquippedThing(45);
			return ((equippedThing != null) ? equippedThing.id : null) == "torch_held";
		}
		case 2:
			return EClass._map.rooms.listLot.Count > 0;
		default:
			return false;
		}
	}

	public override void OnDropReward()
	{
		base.DropReward("housePlate");
		base.DropReward("343");
		base.DropReward("432");
		base.DropReward(ThingGen.CreateRecipe("torch_wall"));
		base.DropReward(ThingGen.CreateRecipe("factory_sign"));
	}
}
