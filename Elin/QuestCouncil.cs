public class QuestCouncil : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		_ = phase;
		return false;
	}
}
