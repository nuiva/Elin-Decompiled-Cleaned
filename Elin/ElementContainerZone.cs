using System;

public class ElementContainerZone : ElementContainer
{
	public override void OnLearn(int ele)
	{
		SE.DingSkill2();
		Msg.Say("learnSkillBranch", EClass._zone.Name, EClass.sources.elements.map[ele].GetName(), null, null);
	}

	public override void OnLevelUp(Element e, int lastValue)
	{
		if (!EClass._zone.IsPCFaction)
		{
			return;
		}
		if (!VirtualDate.IsActive)
		{
			SE.DingSkill2();
			Msg.Say("dingSkillBranch", EClass._zone.Name, e.Name, null, null);
			EClass.pc.pos.TalkWitnesses(EClass.pc, "ding_other", 4, WitnessType.ally, null, 3);
			WidgetPopText.Say("popDing".lang(EClass._zone.Name, e.Name, lastValue.ToString() ?? "", e.vBase.ToString() ?? "", null), FontColor.Good, null);
		}
		EClass.Branch.Log("bDing", EClass._zone.Name, e.Name, lastValue.ToString() ?? "", e.vBase.ToString() ?? "");
	}
}
