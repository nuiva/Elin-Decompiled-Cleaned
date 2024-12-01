public class QuestCompanion : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		return phase switch
		{
			0 => EClass.pc.homeBranch.members.Count >= 3, 
			1 => EClass.pc.homeBranch.members.Count >= 11, 
			_ => false, 
		};
	}

	public override void OnDropReward()
	{
		DropReward(ThingGen.CreateScroll(9001));
	}

	public override string GetTextProgress()
	{
		if (phase == 1)
		{
			return "progressRecruit".lang((EClass.pc.homeBranch.members.Count - 1).ToString() ?? "", 10.ToString() ?? "");
		}
		return "";
	}
}
