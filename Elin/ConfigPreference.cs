using Newtonsoft.Json;

public class ConfigPreference : EClass
{
	[JsonProperty]
	public bool keepPlayingMusic;

	[JsonProperty]
	public bool pickFish;

	[JsonProperty]
	public bool autoEat;
}
