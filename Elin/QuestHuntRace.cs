public class QuestHuntRace : QuestHunt
{
	public override string TextExtra2 => "noDeadLine".lang();

	public override void OnInit()
	{
		SetTask(new QuestTaskHunt
		{
			type = QuestTaskHunt.Type.Race
		});
	}

	public override void OnStart()
	{
		deadline = 0;
	}
}
