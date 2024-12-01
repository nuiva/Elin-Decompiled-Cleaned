using System;
using Newtonsoft.Json;

[Serializable]
public class GuildRankData : EClass
{
	[JsonProperty]
	public string idLang;

	[JsonProperty]
	public int exp;

	[JsonProperty]
	public bool restricted;
}
