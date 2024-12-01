public class ElementContainerZone : ElementContainer
{
	public override void OnLearn(int ele)
	{
		SE.DingSkill2();
		Msg.Say("learnSkillBranch", EClass._zone.Name, EClass.sources.elements.map[ele].GetName());
	}

	public override void OnLevelUp(Element e, int lastValue)
	{
		if (EClass._zone.IsPCFaction)
		{
			if (!VirtualDate.IsActive)
			{
				SE.DingSkill2();
				Msg.Say("dingSkillBranch", EClass._zone.Name, e.Name);
				EClass.pc.pos.TalkWitnesses(EClass.pc, "ding_other", 4, WitnessType.ally);
				WidgetPopText.Say("popDing".lang(EClass._zone.Name, e.Name, lastValue.ToString() ?? "", e.vBase.ToString() ?? ""), FontColor.Good);
			}
			EClass.Branch.Log("bDing", EClass._zone.Name, e.Name, lastValue.ToString() ?? "", e.vBase.ToString() ?? "");
		}
	}
}
