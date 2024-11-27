using System;
using Newtonsoft.Json;

public class HireInfo : EClass
{
	public bool IsExpired
	{
		get
		{
			return this.deadline > 0 && this.Hours < 0;
		}
	}

	public int Hours
	{
		get
		{
			return EClass.world.date.GetRemainingHours(this.deadline);
		}
	}

	public int Days
	{
		get
		{
			if (this.deadline != 0)
			{
				return this.Hours / 24;
			}
			return -1;
		}
	}

	[JsonProperty]
	public Chara chara;

	[JsonProperty]
	public bool isNew;

	[JsonProperty]
	public int deadline;
}
