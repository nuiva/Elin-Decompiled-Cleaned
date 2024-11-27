using System;

public class QuestLoytelFarm : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		if (this.phase == 0)
		{
			if (EClass.pc.things.Find("hoe", -1, -1) == null)
			{
				return false;
			}
			if (EClass.pc.things.Find("shovel", -1, -1) == null)
			{
				return false;
			}
		}
		return true;
	}

	public override void OnComplete()
	{
		base.DropReward(TraitSeed.MakeSeed("pasture").SetNum(5));
		base.DropReward(TraitSeed.MakeSeed("tomato").SetNum(5));
		base.DropReward(TraitSeed.MakeSeed("kinoko").SetNum(5));
		EClass.game.quests.Add("greatDebt", "loytel").startDate = EClass.world.date.GetRaw(0) + 1440;
	}
}
