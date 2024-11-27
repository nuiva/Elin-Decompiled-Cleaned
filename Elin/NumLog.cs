using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class NumLog : EClass
{
	public int maxMonth
	{
		get
		{
			return 8;
		}
	}

	public virtual Gross gross
	{
		get
		{
			return null;
		}
	}

	public virtual string Name
	{
		get
		{
			return "";
		}
	}

	public virtual int Value
	{
		get
		{
			if (this.gross == null)
			{
				return 0;
			}
			return this.gross.Refresh();
		}
	}

	public int lastMonth
	{
		get
		{
			if (this.list.Count <= 0)
			{
				return 0;
			}
			return this.list[0];
		}
	}

	public int lastYear
	{
		get
		{
			if (this.list.Count <= 3)
			{
				return 0;
			}
			return this.list[3];
		}
	}

	public void LogDay()
	{
		this.lastDay = this.Value;
	}

	public void LogMonth()
	{
		this.list.Insert(0, this.Value);
		if (this.list.Count >= this.maxMonth)
		{
			this.list.RemoveAt(this.list.Count - 1);
		}
	}

	[JsonProperty]
	public List<int> list = new List<int>();

	[JsonProperty]
	public int lastDay;
}
