public class QuestIntoDarkness : QuestProgression
{
	public override string TitlePrefix => "★";

	public override void OnStart()
	{
		EClass.game.quests.Add("demitas_spellwriter").startDate = EClass.world.date.GetRaw() + 1440;
	}

	public override bool CanUpdateOnTalk(Chara c)
	{
		return false;
	}
}
