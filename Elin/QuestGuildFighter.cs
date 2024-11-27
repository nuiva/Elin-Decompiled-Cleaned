using System;

public class QuestGuildFighter : QuestGuild
{
	public override Guild guild
	{
		get
		{
			return Guild.Fighter;
		}
	}

	public override void OnInit()
	{
		base.SetTask(new QuestTaskHunt
		{
			setup = QuestTaskHunt.Setup.FighterGuild
		});
	}

	public override void OnChangePhase(int a)
	{
	}
}
