using System;

public class QuestCursedManor : QuestSequence
{
	public override string TitlePrefix
	{
		get
		{
			return "☆";
		}
	}

	public override bool HasDLC
	{
		get
		{
			return Steam.HasDLC(ID_DLC.CursedManor);
		}
	}

	public override bool CanAutoAdvance
	{
		get
		{
			return false;
		}
	}
}
