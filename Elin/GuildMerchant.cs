using System;

public class GuildMerchant : Guild
{
	public override QuestGuild Quest
	{
		get
		{
			return EClass.game.quests.Get<QuestGuildMerchant>();
		}
	}

	public override bool IsCurrentZone
	{
		get
		{
			return EClass._zone.id == "guild_merchant";
		}
	}

	public int InvestPrice(int a)
	{
		if (!base.IsMember)
		{
			return a;
		}
		return (int)((long)a * 100L / (long)(110 + this.relation.rank / 2));
	}
}
