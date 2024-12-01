using Newtonsoft.Json;

public class QuestTaskFlyer : QuestTask
{
	[JsonProperty]
	public int num;

	[JsonProperty]
	public int numRequired;

	public override string RefDrama2 => num.ToString() ?? "";

	public override string RefDrama3 => numRequired.ToString() ?? "";

	public override bool IsComplete()
	{
		return num >= numRequired;
	}

	public override void OnGiveItem(Chara c, Thing t)
	{
		if (t.id == "flyer" && EClass._zone.id != "guild_merchant")
		{
			num++;
		}
	}

	public override void OnInit()
	{
		numRequired = 30;
	}

	public override string GetTextProgress()
	{
		return "progressFlyer".lang(num.ToString() ?? "", numRequired.ToString() ?? "");
	}
}
