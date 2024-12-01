public class QuestGuildMerchant : QuestGuild
{
	public override Guild guild => Guild.Merchant;

	public override void OnInit()
	{
		SetTask(new QuestTaskFlyer());
	}
}
