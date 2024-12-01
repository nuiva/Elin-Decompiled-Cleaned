using System.Collections.Generic;
using Newtonsoft.Json;

public class PartyManager : EClass
{
	[JsonProperty]
	public List<Party> list = new List<Party>();

	public Party Create(Chara leader)
	{
		Party party = new Party();
		list.Add(party);
		party.AddMemeber(leader);
		party.SetLeader(leader);
		return party;
	}
}
