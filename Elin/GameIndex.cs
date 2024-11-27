using System;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptOut)]
public class GameIndex : EClass
{
	public string Title
	{
		get
		{
			return this.factionName ?? "Unknown";
		}
	}

	public string RealDate
	{
		get
		{
			return this.real.GetText(Date.TextFormat.LogPlusYear);
		}
	}

	public string GameData
	{
		get
		{
			return this.date.GetText(Date.TextFormat.LogPlusYear);
		}
	}

	public string FormTitle
	{
		get
		{
			return string.Concat(new string[]
			{
				this.id,
				": ",
				this.zoneName,
				"(",
				this.factionName,
				") ",
				this.RealDate
			});
		}
	}

	public GameIndex Create(Game game)
	{
		if (game != null)
		{
			this.factionName = EClass.Home.name;
			this.zoneName = game.player.zone.Name;
			this.pcName = EClass.pc.c_altName;
			this.aka = EClass.pc.Aka;
			this.date = game.world.date.Copy();
			this.difficulty = game.idDifficulty;
			this.idPortrait = EClass.pc.c_idPortrait;
			this.idRace = EClass.pc.race.id;
			this.idJob = EClass.pc.job.id;
			this.days = EClass.player.stats.days;
			this.deepest = EClass.player.stats.deepest;
			PCCData pccData = EClass.pc.pccData;
			Color color = (pccData != null) ? pccData.GetHairColor(true) : Color.white;
			this.color = ColorUtility.ToHtmlStringRGB(color);
		}
		else
		{
			this.date = new Date();
		}
		this.real = new Date
		{
			year = DateTime.Now.Year,
			month = DateTime.Now.Month,
			day = DateTime.Now.Day,
			hour = DateTime.Now.Hour,
			min = DateTime.Now.Minute
		};
		this.version = EClass.core.version;
		return this;
	}

	public Date date;

	public Date real;

	public string id;

	public string color;

	public int difficulty;

	public int days;

	public int deepest;

	public global::Version version;

	public string zoneName;

	public string factionName;

	public string pcName;

	public string aka;

	public string idPortrait;

	public string idRace;

	public string idJob;

	public bool madeBackup;

	public bool isBackup;

	[JsonIgnore]
	public string path;
}
