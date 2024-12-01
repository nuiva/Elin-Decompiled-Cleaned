using Newtonsoft.Json;

public class HireInfo : EClass
{
	[JsonProperty]
	public Chara chara;

	[JsonProperty]
	public bool isNew;

	[JsonProperty]
	public int deadline;

	public bool IsExpired
	{
		get
		{
			if (deadline > 0)
			{
				return Hours < 0;
			}
			return false;
		}
	}

	public int Hours => EClass.world.date.GetRemainingHours(deadline);

	public int Days
	{
		get
		{
			if (deadline != 0)
			{
				return Hours / 24;
			}
			return -1;
		}
	}
}
