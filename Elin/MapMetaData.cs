using System;
using Newtonsoft.Json;

public class MapMetaData : EClass
{
	public bool IsValidVersion()
	{
		return !global::Version.Get(this.version).IsBelow(EClass.core.versionMoongate);
	}

	[JsonProperty]
	public string name;

	[JsonProperty]
	public string id;

	[JsonProperty]
	public string type;

	[JsonProperty]
	public int version;

	[JsonProperty]
	public PartialMap partial;

	public string path;
}
