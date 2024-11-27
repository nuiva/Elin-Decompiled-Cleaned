using System;
using Newtonsoft.Json;

[Serializable]
public class GodStatueData : EClass
{
	[JsonProperty]
	public string id;

	[JsonProperty]
	public string idThing;

	[JsonProperty]
	public float chance;
}
