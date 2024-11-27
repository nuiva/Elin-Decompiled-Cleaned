using System;

public class QuestHuntRace : QuestHunt
{
	public override string TextExtra2
	{
		get
		{
			return "noDeadLine".lang();
		}
	}

	public override void OnInit()
	{
		base.SetTask(new QuestTaskHunt
		{
			type = QuestTaskHunt.Type.Race
		});
	}

	public override void OnStart()
	{
		this.deadline = 0;
	}
}
