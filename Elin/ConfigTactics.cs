using Newtonsoft.Json;

public class ConfigTactics : EClass
{
	[JsonProperty]
	public bool allyKeepDistance;

	[JsonProperty]
	public bool dontWander;

	public int AllyDistance(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			return 1;
		}
		if (allyKeepDistance && EClass._zone.KeepAllyDistance)
		{
			return 5;
		}
		return c.DestDist;
	}
}
