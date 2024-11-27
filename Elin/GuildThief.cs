using System;

public class GuildThief : Guild
{
	public override QuestGuild Quest
	{
		get
		{
			return EClass.game.quests.Get<QuestGuildThief>();
		}
	}

	public override bool IsCurrentZone
	{
		get
		{
			return EClass._zone.id == "derphy" && EClass._zone.lv == -1;
		}
	}

	public int SellStolenPrice(int a)
	{
		if (!base.IsMember)
		{
			return a;
		}
		return a * 100 / (190 - this.relation.rank * 2);
	}
}
