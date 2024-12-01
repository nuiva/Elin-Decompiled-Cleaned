using System.Collections.Generic;
using Newtonsoft.Json;

public class Schedule
{
	public class Item
	{
		[JsonProperty]
		public Date date;

		public string Name = "tempSchedule".lang();
	}

	[JsonProperty]
	public List<Item> list = new List<Item>();
}
