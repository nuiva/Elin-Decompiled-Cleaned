public class GuildMerchant : Guild
{
	public override QuestGuild Quest => EClass.game.quests.Get<QuestGuildMerchant>();

	public override bool IsCurrentZone => EClass._zone.id == "guild_merchant";

	public int InvestPrice(int a)
	{
		if (!base.IsMember)
		{
			return a;
		}
		return (int)((long)a * 100L / (110 + relation.rank / 2));
	}
}
