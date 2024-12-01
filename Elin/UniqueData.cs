using Newtonsoft.Json;

public class UniqueData : EClass
{
	[JsonProperty]
	public int x;

	[JsonProperty]
	public int y;

	[JsonProperty]
	public int uidZone;
}
