public class QuestLoytelFarm : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		if (phase == 0)
		{
			if (EClass.pc.things.Find("hoe") == null)
			{
				return false;
			}
			if (EClass.pc.things.Find("shovel") == null)
			{
				return false;
			}
		}
		return true;
	}

	public override void OnComplete()
	{
		DropReward(TraitSeed.MakeSeed("pasture").SetNum(5));
		DropReward(TraitSeed.MakeSeed("tomato").SetNum(5));
		DropReward(TraitSeed.MakeSeed("kinoko").SetNum(5));
		EClass.game.quests.Add("greatDebt", "loytel").startDate = EClass.world.date.GetRaw() + 1440;
	}
}
