using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class FactionManager : EClass
{
	[JsonProperty]
	public Dictionary<string, Faction> dictAll = new Dictionary<string, Faction>();

	[JsonProperty]
	public new Faction Home;

	[JsonProperty]
	public new Faction Wilds;

	[JsonProperty]
	public GuildFighter Fighter;

	[JsonProperty]
	public GuildMage Mage;

	[JsonProperty]
	public GuildThief Thief;

	[JsonProperty]
	public GuildMerchant Merchant;

	public void AssignUID(Faction s)
	{
		Faction faction = dictAll.TryGetValue(s.id);
		if (faction != null)
		{
			Debug.LogError("exception: Faction id already exists:" + faction.id);
		}
		s.uid = s.id;
		dictAll.Add(s.uid, s);
	}

	public void OnCreateGame()
	{
		foreach (SourceFaction.Row row in EClass.sources.factions.rows)
		{
			Faction.Create(row);
		}
		Home = Find("home");
		Home.relation.type = FactionRelation.RelationType.Owner;
		Wilds = Find("wilds");
		Fighter = Find<GuildFighter>("guild_fighter");
		Mage = Find<GuildMage>("guild_mage");
		Thief = Find<GuildThief>("guild_thief");
		Merchant = Find<GuildMerchant>("guild_merchant");
	}

	public void OnLoad()
	{
		foreach (Faction value in dictAll.Values)
		{
			value.OnLoad();
		}
	}

	public T Find<T>(string id) where T : Faction
	{
		return Find(id) as T;
	}

	public Faction Find(string id)
	{
		foreach (Faction value in dictAll.Values)
		{
			if (value.id == id)
			{
				return value;
			}
		}
		return null;
	}
}
