using System;

public class QuestCouncil : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		int phase = this.phase;
		return false;
	}
}
