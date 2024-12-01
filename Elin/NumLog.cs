using System.Collections.Generic;
using Newtonsoft.Json;

public class NumLog : EClass
{
	[JsonProperty]
	public List<int> list = new List<int>();

	[JsonProperty]
	public int lastDay;

	public int maxMonth => 8;

	public virtual Gross gross => null;

	public virtual string Name => "";

	public virtual int Value
	{
		get
		{
			if (gross == null)
			{
				return 0;
			}
			return gross.Refresh();
		}
	}

	public int lastMonth
	{
		get
		{
			if (list.Count <= 0)
			{
				return 0;
			}
			return list[0];
		}
	}

	public int lastYear
	{
		get
		{
			if (list.Count <= 3)
			{
				return 0;
			}
			return list[3];
		}
	}

	public void LogDay()
	{
		lastDay = Value;
	}

	public void LogMonth()
	{
		list.Insert(0, Value);
		if (list.Count >= maxMonth)
		{
			list.RemoveAt(list.Count - 1);
		}
	}
}
