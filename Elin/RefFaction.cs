using Newtonsoft.Json;

public class RefFaction : EClass
{
	[JsonProperty]
	public string uid;

	private Faction _faction;

	public Faction Instance => _faction ?? (_faction = EClass.game.factions.dictAll[uid]);

	public RefFaction()
	{
	}

	public RefFaction(Faction faction)
	{
		_faction = faction;
		uid = faction.uid;
	}
}
