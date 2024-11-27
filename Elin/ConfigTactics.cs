using System;
using Newtonsoft.Json;

public class ConfigTactics : EClass
{
	public int AllyDistance(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			return 1;
		}
		if (this.allyKeepDistance && EClass._zone.KeepAllyDistance)
		{
			return 5;
		}
		return c.DestDist;
	}

	[JsonProperty]
	public bool allyKeepDistance;

	[JsonProperty]
	public bool dontWander;
}
