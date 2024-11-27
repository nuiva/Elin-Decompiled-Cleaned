using System;

public class QuestMain : QuestSequence
{
	public override string TitlePrefix
	{
		get
		{
			return "★";
		}
	}

	public override string idSource
	{
		get
		{
			return this.id;
		}
	}

	public static int Phase
	{
		get
		{
			QuestMain questMain = EClass.game.quests.Get<QuestMain>();
			if (questMain == null)
			{
				return 0;
			}
			return questMain.phase;
		}
	}

	public override bool CanAutoAdvance
	{
		get
		{
			return false;
		}
	}

	public const int Started = 0;

	public const int AfterMeetAsh = 100;

	public const int AfterReadDeed = 200;

	public const int AfterReportAsh = 250;

	public const int AfterMeetFarris = 300;

	public const int AfterNymelle = 600;

	public const int AfterAshLeaveHome = 700;
}
