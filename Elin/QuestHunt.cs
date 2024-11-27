using System;

public class QuestHunt : QuestRandom
{
	public override int KarmaOnFail
	{
		get
		{
			return -3;
		}
	}

	public override string RewardSuffix
	{
		get
		{
			return "Hunt";
		}
	}

	public override int RangeDeadLine
	{
		get
		{
			return 20;
		}
	}

	public override void OnInit()
	{
		base.SetTask(new QuestTaskHunt());
	}
}
