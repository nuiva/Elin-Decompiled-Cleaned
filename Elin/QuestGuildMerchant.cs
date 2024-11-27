using System;

public class QuestGuildMerchant : QuestGuild
{
	public override Guild guild
	{
		get
		{
			return Guild.Merchant;
		}
	}

	public override void OnInit()
	{
		base.SetTask(new QuestTaskFlyer());
	}
}
