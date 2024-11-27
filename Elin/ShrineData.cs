using System;
using Newtonsoft.Json;

[Serializable]
public class ShrineData : EClass
{
	[JsonProperty]
	public string id;

	[JsonProperty]
	public float chance;

	[JsonProperty]
	public int tile;

	[JsonProperty]
	public int skin;
}
