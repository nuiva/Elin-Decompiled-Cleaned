using System;

public class Guild : Faction
{
	public static Guild Current
	{
		get
		{
			if (EClass._zone.id == "guild_merchant")
			{
				return EClass.game.factions.Merchant;
			}
			if (EClass._zone.id == "lumiest")
			{
				return EClass.game.factions.Mage;
			}
			if (!(EClass._zone.id == "derphy"))
			{
				return EClass.game.factions.Fighter;
			}
			return EClass.game.factions.Thief;
		}
	}

	public static GuildFighter Fighter
	{
		get
		{
			return EClass.game.factions.Fighter;
		}
	}

	public static GuildMage Mage
	{
		get
		{
			return EClass.game.factions.Mage;
		}
	}

	public static GuildThief Thief
	{
		get
		{
			return EClass.game.factions.Thief;
		}
	}

	public static GuildMerchant Merchant
	{
		get
		{
			return EClass.game.factions.Merchant;
		}
	}

	public static QuestGuild CurrentQuest
	{
		get
		{
			Guild guild = Guild.Current;
			if (guild == null)
			{
				return null;
			}
			return guild.Quest;
		}
	}

	public static Guild GetCurrentGuild()
	{
		if (Guild.Fighter.IsCurrentZone)
		{
			return Guild.Fighter;
		}
		if (Guild.Mage.IsCurrentZone)
		{
			return Guild.Mage;
		}
		if (Guild.Thief.IsCurrentZone)
		{
			return Guild.Thief;
		}
		if (Guild.Merchant.IsCurrentZone)
		{
			return Guild.Merchant;
		}
		return null;
	}

	public void RefreshDevelopment()
	{
		EClass._zone.development = (10 + this.relation.rank * 5) * 10;
	}

	public override string TextType
	{
		get
		{
			return "sub_guild".lang();
		}
	}

	public virtual QuestGuild Quest
	{
		get
		{
			return null;
		}
	}

	public virtual bool IsCurrentZone
	{
		get
		{
			return false;
		}
	}

	public bool IsMember
	{
		get
		{
			return this.relation.type == FactionRelation.RelationType.Member;
		}
	}
}
