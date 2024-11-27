using System;
using Newtonsoft.Json;

public class QuestTaskKarma : QuestTask
{
	public override string RefDrama3
	{
		get
		{
			return this.numRequired.ToString() ?? "";
		}
	}

	public override bool IsComplete()
	{
		if (this.vec != 1)
		{
			return this.num <= this.numRequired;
		}
		return this.num >= this.numRequired;
	}

	public override void OnInit()
	{
		if (this.setup == QuestTaskKarma.Setup.ThiefGuild)
		{
			this.numRequired = -100;
			this.vec = -1;
			return;
		}
		this.numRequired = 10 + EClass.rnd(10);
		this.vec = 1;
	}

	public override void OnModKarma(int a)
	{
		if (this.vec == 1 && a > 0)
		{
			this.num += a;
		}
		if (this.vec == -1 && a < 0)
		{
			this.num += a;
		}
	}

	public override string GetTextProgress()
	{
		return "progressKarma".lang(this.num.ToString() ?? "", this.numRequired.ToString() ?? "", null, null, null);
	}

	[JsonProperty]
	public int num;

	[JsonProperty]
	public int numRequired;

	[JsonProperty]
	public int vec;

	public QuestTaskKarma.Setup setup;

	public enum Setup
	{
		Random,
		ThiefGuild
	}
}
