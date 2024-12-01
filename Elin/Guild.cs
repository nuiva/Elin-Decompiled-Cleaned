public class Guild : Faction
{
	public static Guild Current
	{
		get
		{
			if (!(EClass._zone.id == "guild_merchant"))
			{
				if (!(EClass._zone.id == "lumiest"))
				{
					if (!(EClass._zone.id == "derphy"))
					{
						return EClass.game.factions.Fighter;
					}
					return EClass.game.factions.Thief;
				}
				return EClass.game.factions.Mage;
			}
			return EClass.game.factions.Merchant;
		}
	}

	public static GuildFighter Fighter => EClass.game.factions.Fighter;

	public static GuildMage Mage => EClass.game.factions.Mage;

	public static GuildThief Thief => EClass.game.factions.Thief;

	public static GuildMerchant Merchant => EClass.game.factions.Merchant;

	public static QuestGuild CurrentQuest => Current?.Quest;

	public override string TextType => "sub_guild".lang();

	public virtual QuestGuild Quest => null;

	public virtual bool IsCurrentZone => false;

	public bool IsMember => relation.type == FactionRelation.RelationType.Member;

	public static Guild GetCurrentGuild()
	{
		if (Fighter.IsCurrentZone)
		{
			return Fighter;
		}
		if (Mage.IsCurrentZone)
		{
			return Mage;
		}
		if (Thief.IsCurrentZone)
		{
			return Thief;
		}
		if (Merchant.IsCurrentZone)
		{
			return Merchant;
		}
		return null;
	}

	public void RefreshDevelopment()
	{
		EClass._zone.development = (10 + relation.rank * 5) * 10;
	}
}
