using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class ReligionManager : EClass
{
	public void SetOwner()
	{
		this.list = new List<Religion>
		{
			this.Eyth,
			this.Wind,
			this.Earth,
			this.Healing,
			this.Luck,
			this.Machine,
			this.Element,
			this.Harvest,
			this.Oblivion,
			this.Harmony,
			this.Trickery,
			this.MoonShadow,
			this.Strife
		};
		foreach (Religion religion in this.list)
		{
			this.dictAll.Add(religion.id, religion);
		}
	}

	public void OnCreateGame()
	{
		this.SetOwner();
		foreach (Religion religion in this.list)
		{
			religion.Init();
		}
	}

	public void OnLoad()
	{
		this.SetOwner();
		foreach (Religion religion in this.dictAll.Values)
		{
			religion.OnLoad();
		}
	}

	public Religion Find(string id)
	{
		foreach (Religion religion in this.dictAll.Values)
		{
			if (religion.id == id)
			{
				return religion;
			}
		}
		return null;
	}

	public Religion GetRandomReligion(bool onlyJoinable = true, bool includeMinor = false)
	{
		return (from a in this.list
		where (!onlyJoinable || a.CanJoin) && (includeMinor || !a.IsMinorGod)
		select a).RandomItem<Religion>();
	}

	public Dictionary<string, Religion> dictAll = new Dictionary<string, Religion>();

	public List<Religion> list = new List<Religion>();

	[JsonProperty]
	public ReligionEyth Eyth = new ReligionEyth();

	[JsonProperty]
	public ReligionWind Wind = new ReligionWind();

	[JsonProperty]
	public ReligionEarth Earth = new ReligionEarth();

	[JsonProperty]
	public ReligionHealing Healing = new ReligionHealing();

	[JsonProperty]
	public ReligionLuck Luck = new ReligionLuck();

	[JsonProperty]
	public ReligionMachine Machine = new ReligionMachine();

	[JsonProperty]
	public ReligionElement Element = new ReligionElement();

	[JsonProperty]
	public ReligionHarvest Harvest = new ReligionHarvest();

	[JsonProperty]
	public ReligionOblivion Oblivion = new ReligionOblivion();

	[JsonProperty]
	public ReligionHarmony Harmony = new ReligionHarmony();

	[JsonProperty]
	public ReligionTrickery Trickery = new ReligionTrickery();

	[JsonProperty]
	public ReligionMoonShadow MoonShadow = new ReligionMoonShadow();

	[JsonProperty]
	public ReligionStrife Strife = new ReligionStrife();
}
