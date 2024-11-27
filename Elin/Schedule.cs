using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Schedule
{
	[JsonProperty]
	public List<Schedule.Item> list = new List<Schedule.Item>();

	public class Item
	{
		[JsonProperty]
		public Date date;

		public string Name = "tempSchedule".lang();
	}
}
