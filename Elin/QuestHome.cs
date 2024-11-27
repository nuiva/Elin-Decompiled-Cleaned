using System;

public class QuestHome : QuestSequence
{
	public override string TitlePrefix
	{
		get
		{
			return "★";
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

	public const int AfterReadDeed = 1;

	public const int AfterReportAsh = 2;
}
