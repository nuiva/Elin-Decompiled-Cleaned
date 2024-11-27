using System;
using UnityEngine;

public class QuestGuild : QuestSequence
{
	public virtual Guild guild
	{
		get
		{
			return null;
		}
	}

	public override string TitlePrefix
	{
		get
		{
			return "☆";
		}
	}

	public override bool UpdateOnTalk()
	{
		return false;
	}

	public override string GetTrackerText()
	{
		if (this.guild.relation.type == FactionRelation.RelationType.Member)
		{
			FactionRelation relation = this.guild.relation;
			return "faction_contribution".lang(Mathf.Min(relation.exp, relation.ExpToNext).ToString() ?? "", relation.ExpToNext.ToString() ?? "", null, null, null);
		}
		return base.GetTrackerText();
	}

	public override string GetDetailText(bool onJournal = false)
	{
		string text = base.GetDetailText(onJournal);
		if (this.guild.relation.type == FactionRelation.RelationType.Member)
		{
			FactionRelation relation = this.guild.relation;
			int num = relation.rank / 2;
			text += "\n\n";
			text = text + this.guild.Name.TagColor(FontColor.Topic, null) + "\n";
			text = text + "faction_rank".lang(relation.rank.ToString() ?? "", relation.TextTitle, null, null, null) + "\n";
			text = text + "faction_contribution".lang(Mathf.Min(relation.exp, relation.ExpToNext).ToString() ?? "", relation.ExpToNext.ToString() ?? "", null, null, null) + "\n";
			text = text + "faction_salary".lang(relation.GetSalary().ToString() ?? "", null, null, null, null) + "\n";
			text += "\n";
			text = text + "faction_task".lang().TagColor(FontColor.Topic, null) + "\n";
			text = text + (this.guild.id + "_task1").lang() + "\n";
			text += "\n";
			text = text + "faction_benefit".lang().TagColor(FontColor.Topic, null) + "\n";
			text = text + (this.guild.id + "_benefit1").lang() + "\n";
			if (num >= 1)
			{
				text = text + "guild_benefit_trainer".lang() + "\n";
			}
			if (num >= 2)
			{
				text = text + (this.guild.id + "_benefit2").lang() + "\n";
			}
			if (num >= 4)
			{
				text = text + "guild_benefit_feat".lang() + "\n";
			}
		}
		return text;
	}

	public const int Started = 0;

	public const int CompletedTrial = 1;

	public const int Joined = 10;
}
