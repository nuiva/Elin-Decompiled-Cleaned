using Newtonsoft.Json;

public class PlantData
{
	[JsonProperty]
	public Thing seed;

	[JsonProperty]
	public int water;

	[JsonProperty]
	public int fert;

	[JsonProperty]
	public int size;
}
