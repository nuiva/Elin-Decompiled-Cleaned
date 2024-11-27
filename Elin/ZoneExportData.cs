using System;
using Newtonsoft.Json;

public class ZoneExportData : EClass
{
	[JsonProperty]
	public string name;

	[JsonProperty]
	public SerializedCards serializedCards = new SerializedCards();

	[JsonProperty]
	public bool usermap;

	public Map orgMap;
}
