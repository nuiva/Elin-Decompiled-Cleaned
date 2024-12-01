using Newtonsoft.Json;

public class GlobalData : EClass
{
	[JsonProperty]
	public ZoneTransition transition;

	[JsonProperty]
	public GlobalGoal goal;
}
