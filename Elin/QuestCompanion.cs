using System;

public class QuestCompanion : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		int phase = this.phase;
		if (phase != 0)
		{
			return phase == 1 && EClass.pc.homeBranch.members.Count >= 11;
		}
		return EClass.pc.homeBranch.members.Count >= 3;
	}

	public override void OnDropReward()
	{
		base.DropReward(ThingGen.CreateScroll(9001, 1));
	}

	public override string GetTextProgress()
	{
		if (this.phase == 1)
		{
			return "progressRecruit".lang((EClass.pc.homeBranch.members.Count - 1).ToString() ?? "", 10.ToString() ?? "", null, null, null);
		}
		return "";
	}
}
