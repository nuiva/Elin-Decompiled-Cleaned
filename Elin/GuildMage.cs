using System;

public class GuildMage : Guild
{
	public override QuestGuild Quest
	{
		get
		{
			return EClass.game.quests.Get<QuestGuildMage>();
		}
	}

	public override bool IsCurrentZone
	{
		get
		{
			return EClass._zone.id == "lumiest" && EClass._zone.lv == -1;
		}
	}

	public int BuySpellbookPrice(int a)
	{
		if (!base.IsMember)
		{
			return a;
		}
		return a * 100 / (120 + this.relation.rank / 2);
	}
}
