public class QuestGuildThief : QuestGuild
{
	public override Guild guild => Guild.Thief;

	public override void OnInit()
	{
		SetTask(new QuestTaskKarma
		{
			setup = QuestTaskKarma.Setup.ThiefGuild
		});
	}
}
