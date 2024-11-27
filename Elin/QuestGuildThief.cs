using System;

public class QuestGuildThief : QuestGuild
{
	public override Guild guild
	{
		get
		{
			return Guild.Thief;
		}
	}

	public override void OnInit()
	{
		base.SetTask(new QuestTaskKarma
		{
			setup = QuestTaskKarma.Setup.ThiefGuild
		});
	}
}
