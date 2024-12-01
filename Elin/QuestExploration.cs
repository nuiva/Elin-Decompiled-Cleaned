public class QuestExploration : QuestProgression
{
	public const int Started = 0;

	public const int AfterMeetFarris = 1;

	public const int AfterMeetFarrisAtHome = 2;

	public const int AfterDefeatNymelleBoss = 3;

	public const int AfterCrystalTalk = 5;

	public const int AfterNymelle = 6;

	public override bool CanAutoAdvance => false;

	public override void OnStart()
	{
		EClass.game.quests.globalList.Add(Quest.Create("fiama_reward").SetClient(EClass.game.cards.globalCharas.Find("fiama"), assignQuest: false));
	}

	public override bool CanUpdateOnTalk(Chara c)
	{
		if (c.id == "ashland" && phase == 5)
		{
			return true;
		}
		return false;
	}
}
