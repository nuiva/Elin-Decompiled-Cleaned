using System;
using Newtonsoft.Json;

public class RefFaction : EClass
{
	public Faction Instance
	{
		get
		{
			Faction result;
			if ((result = this._faction) == null)
			{
				result = (this._faction = EClass.game.factions.dictAll[this.uid]);
			}
			return result;
		}
	}

	public RefFaction()
	{
	}

	public RefFaction(Faction faction)
	{
		this._faction = faction;
		this.uid = faction.uid;
	}

	[JsonProperty]
	public string uid;

	private Faction _faction;
}
