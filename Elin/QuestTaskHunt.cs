using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

public class QuestTaskHunt : QuestTask
{
	public SourceRace.Row Race
	{
		get
		{
			return EClass.sources.races.map[this.idRace];
		}
	}

	public override string RefDrama2
	{
		get
		{
			if (this.type != QuestTaskHunt.Type.Race)
			{
				return "";
			}
			return this.Race.GetName();
		}
	}

	public override string RefDrama3
	{
		get
		{
			return this.numRequired.ToString() ?? "";
		}
	}

	public override bool IsComplete()
	{
		return this.numHunted >= this.numRequired;
	}

	public static List<SourceChara.Row> ListTargets(string idRace)
	{
		List<SourceChara.Row> list = new List<SourceChara.Row>();
		foreach (SourceChara.Row row in EClass.sources.charas.rows)
		{
			if (row.quality == 0 && row.race == idRace && row.chance > 0 && row.hostility != "Friend")
			{
				list.Add(row);
			}
		}
		return list;
	}

	public override void OnInit()
	{
		if (this.setup == QuestTaskHunt.Setup.FighterGuild)
		{
			this.numRequired = 20;
			this.idRace = "yeek";
			this.type = QuestTaskHunt.Type.Race;
			return;
		}
		if (this.type == QuestTaskHunt.Type.Race)
		{
			for (int i = 0; i < 100; i++)
			{
				SourceRace.Row row = EClass.sources.races.rows.RandomItem<SourceRace.Row>();
				if (QuestTaskHunt.ListTargets(row.id).Count != 0)
				{
					this.idRace = row.id;
				}
			}
			this.numRequired = 3 + this.owner.difficulty * 2 + EClass.rnd(5);
			return;
		}
		this.numRequired = 10 + this.owner.difficulty * 3 + EClass.rnd(5);
	}

	public override void OnKillChara(Chara c)
	{
		QuestTaskHunt.<>c__DisplayClass16_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.c = c;
		if (CS$<>8__locals1.c.IsPCFaction)
		{
			return;
		}
		if (this.type == QuestTaskHunt.Type.Race)
		{
			if (CS$<>8__locals1.c.race.id == this.idRace)
			{
				this.<OnKillChara>g__CountKill|16_0(ref CS$<>8__locals1);
				return;
			}
		}
		else if (CS$<>8__locals1.c.OriginalHostility <= Hostility.Enemy)
		{
			this.<OnKillChara>g__CountKill|16_0(ref CS$<>8__locals1);
		}
	}

	public override string GetTextProgress()
	{
		if (this.type == QuestTaskHunt.Type.Race)
		{
			return "progressHuntRace".lang(this.numHunted.ToString() ?? "", this.numRequired.ToString() ?? "", this.Race.GetName(), null, null);
		}
		return "progressHunt".lang(this.numHunted.ToString() ?? "", this.numRequired.ToString() ?? "", null, null, null);
	}

	public override void OnGetDetail(ref string detail, bool onJournal)
	{
		if (this.type == QuestTaskHunt.Type.Race)
		{
			if (!onJournal)
			{
				return;
			}
			List<SourceChara.Row> list = QuestTaskHunt.ListTargets(this.idRace);
			int num = 0;
			detail = string.Concat(new string[]
			{
				detail,
				Environment.NewLine,
				Environment.NewLine,
				"target_huntRace".lang(),
				Environment.NewLine
			});
			foreach (SourceChara.Row row in list)
			{
				detail = string.Concat(new string[]
				{
					detail,
					row.GetName().ToTitleCase(true),
					" (",
					EClass.sources.races.map[this.idRace].GetName(),
					")"
				});
				num++;
				if (num > 5)
				{
					break;
				}
				detail += Environment.NewLine;
			}
		}
	}

	[CompilerGenerated]
	private void <OnKillChara>g__CountKill|16_0(ref QuestTaskHunt.<>c__DisplayClass16_0 A_1)
	{
		this.numHunted++;
		if (this.numHunted > this.numRequired)
		{
			this.numHunted = this.numRequired;
			return;
		}
		this.owner.bonusMoney += EClass.curve(3 + A_1.c.LV, 50, 10, 75) * ((this.type == QuestTaskHunt.Type.Race) ? 2 : 1);
	}

	[JsonProperty]
	public int numHunted;

	[JsonProperty]
	public int numRequired;

	[JsonProperty]
	public string idRace;

	[JsonProperty]
	public QuestTaskHunt.Type type;

	public QuestTaskHunt.Setup setup;

	public enum Type
	{
		Default,
		Race
	}

	public enum Setup
	{
		Random,
		FighterGuild
	}
}
