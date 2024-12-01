using Newtonsoft.Json;

public class QuestTaskKarma : QuestTask
{
	public enum Setup
	{
		Random,
		ThiefGuild
	}

	[JsonProperty]
	public int num;

	[JsonProperty]
	public int numRequired;

	[JsonProperty]
	public int vec;

	public Setup setup;

	public override string RefDrama3 => numRequired.ToString() ?? "";

	public override bool IsComplete()
	{
		if (vec != 1)
		{
			return num <= numRequired;
		}
		return num >= numRequired;
	}

	public override void OnInit()
	{
		if (setup == Setup.ThiefGuild)
		{
			numRequired = -100;
			vec = -1;
		}
		else
		{
			numRequired = 10 + EClass.rnd(10);
			vec = 1;
		}
	}

	public override void OnModKarma(int a)
	{
		if (vec == 1 && a > 0)
		{
			num += a;
		}
		if (vec == -1 && a < 0)
		{
			num += a;
		}
	}

	public override string GetTextProgress()
	{
		return "progressKarma".lang(num.ToString() ?? "", numRequired.ToString() ?? "");
	}
}
