public class QuestCrafter : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		return phase switch
		{
			0 => EClass._map.Installed.Find("workbench") != null, 
			1 => EClass.pc.body.GetEquippedThing(45)?.id == "torch_held", 
			2 => EClass._map.rooms.listLot.Count > 0, 
			_ => false, 
		};
	}

	public override void OnDropReward()
	{
		DropReward("housePlate");
		DropReward("343");
		DropReward("432");
		DropReward(ThingGen.CreateRecipe("torch_wall"));
		DropReward(ThingGen.CreateRecipe("factory_sign"));
	}
}
