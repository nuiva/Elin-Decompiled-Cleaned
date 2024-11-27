using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class FactionManager : EClass
{
	public void AssignUID(Faction s)
	{
		Faction faction = this.dictAll.TryGetValue(s.id, null);
		if (faction != null)
		{
			Debug.LogError("exception: Faction id already exists:" + faction.id);
		}
		s.uid = s.id;
		this.dictAll.Add(s.uid, s);
	}

	public void OnCreateGame()
	{
		foreach (SourceFaction.Row r in EClass.sources.factions.rows)
		{
			Faction.Create(r);
		}
		this.Home = this.Find("home");
		this.Home.relation.type = FactionRelation.RelationType.Owner;
		this.Wilds = this.Find("wilds");
		this.Fighter = this.Find<GuildFighter>("guild_fighter");
		this.Mage = this.Find<GuildMage>("guild_mage");
		this.Thief = this.Find<GuildThief>("guild_thief");
		this.Merchant = this.Find<GuildMerchant>("guild_merchant");
	}

	public void OnLoad()
	{
		foreach (Faction faction in this.dictAll.Values)
		{
			faction.OnLoad();
		}
	}

	public T Find<T>(string id) where T : Faction
	{
		return this.Find(id) as T;
	}

	public Faction Find(string id)
	{
		foreach (Faction faction in this.dictAll.Values)
		{
			if (faction.id == id)
			{
				return faction;
			}
		}
		return null;
	}

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
}
