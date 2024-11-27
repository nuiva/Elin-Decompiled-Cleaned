using System;

public class QuestExploration : QuestProgression
{
	public override bool CanAutoAdvance
	{
		get
		{
			return false;
		}
	}

	public override void OnStart()
	{
		EClass.game.quests.globalList.Add(Quest.Create("fiama_reward", null, null).SetClient(EClass.game.cards.globalCharas.Find("fiama"), false));
	}

	public override bool CanUpdateOnTalk(Chara c)
	{
		return c.id == "ashland" && this.phase == 5;
	}

	public const int Started = 0;

	public const int AfterMeetFarris = 1;

	public const int AfterMeetFarrisAtHome = 2;

	public const int AfterDefeatNymelleBoss = 3;

	public const int AfterCrystalTalk = 5;

	public const int AfterNymelle = 6;
}
