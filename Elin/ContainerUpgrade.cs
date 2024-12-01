using Newtonsoft.Json;

public class ContainerUpgrade : EClass
{
	[JsonProperty]
	public int cap;

	[JsonProperty]
	public int cool;
}
