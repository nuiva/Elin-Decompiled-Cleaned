using System;
using Newtonsoft.Json;

public class QuestTaskFlyer : QuestTask
{
	public override string RefDrama2
	{
		get
		{
			return this.num.ToString() ?? "";
		}
	}

	public override string RefDrama3
	{
		get
		{
			return this.numRequired.ToString() ?? "";
		}
	}

	public override bool IsComplete()
	{
		return this.num >= this.numRequired;
	}

	public override void OnGiveItem(Chara c, Thing t)
	{
		if (t.id == "flyer" && EClass._zone.id != "guild_merchant")
		{
			this.num++;
		}
	}

	public override void OnInit()
	{
		this.numRequired = 30;
	}

	public override string GetTextProgress()
	{
		return "progressFlyer".lang(this.num.ToString() ?? "", this.numRequired.ToString() ?? "", null, null, null);
	}

	[JsonProperty]
	public int num;

	[JsonProperty]
	public int numRequired;
}
