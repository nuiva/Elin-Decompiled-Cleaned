using System;

public class GuildFighter : Guild
{
	public override QuestGuild Quest
	{
		get
		{
			return EClass.game.quests.Get<QuestGuildFighter>();
		}
	}

	public override bool IsCurrentZone
	{
		get
		{
			return EClass._zone.id == "kapul" && EClass._zone.lv == -1;
		}
	}

	public int ServicePrice(int a)
	{
		if (!base.IsMember)
		{
			return a;
		}
		return a * 100 / (125 + this.relation.rank * 2);
	}
}
