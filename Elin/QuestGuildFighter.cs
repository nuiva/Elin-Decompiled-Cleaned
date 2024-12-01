public class QuestGuildFighter : QuestGuild
{
	public override Guild guild => Guild.Fighter;

	public override void OnInit()
	{
		SetTask(new QuestTaskHunt
		{
			setup = QuestTaskHunt.Setup.FighterGuild
		});
	}

	public override void OnChangePhase(int a)
	{
		_ = 10;
	}
}
