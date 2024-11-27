using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class PartyManager : EClass
{
	public Party Create(Chara leader)
	{
		Party party = new Party();
		this.list.Add(party);
		party.AddMemeber(leader);
		party.SetLeader(leader);
		return party;
	}

	[JsonProperty]
	public List<Party> list = new List<Party>();
}
