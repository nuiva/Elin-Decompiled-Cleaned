using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class MsgLog : EClass
{
	public int maxLog
	{
		get
		{
			if (this.id == "chronicle")
			{
				return 9999;
			}
			return 50;
		}
	}

	public void Add(MsgLog.Data data)
	{
		this.dict.Add(this.currentLogIndex, data);
		this.currentLogIndex++;
		if (this.currentLogIndex >= this.maxLog)
		{
			this.dict.Remove(this.currentLogIndex - this.maxLog);
		}
	}

	public MsgLog.Data Add(string text, FontColor c)
	{
		return this.Add(text, c.ToString());
	}

	public MsgLog.Data Add(string text, string col = null)
	{
		MsgLog.Data data = new MsgLog.Data
		{
			text = text,
			col = col
		};
		data.date = (VirtualDate.current ?? EClass.world.date).Copy();
		this.Add(data);
		return data;
	}

	public List<MsgLog.Data> GetList(bool reverse = false)
	{
		List<MsgLog.Data> list = new List<MsgLog.Data>();
		foreach (MsgLog.Data item in this.dict.Values)
		{
			list.Add(item);
		}
		list.Sort((MsgLog.Data a, MsgLog.Data b) => a.date.GetRaw(0) - b.date.GetRaw(0));
		if (reverse)
		{
			list.Reverse();
		}
		return list;
	}

	[JsonProperty]
	public Dictionary<int, MsgLog.Data> dict = new Dictionary<int, MsgLog.Data>();

	[JsonProperty]
	public int currentLogIndex;

	[JsonProperty]
	public string id;

	public class Data : EClass
	{
		[JsonProperty]
		public string text;

		[JsonProperty]
		public string col;

		[JsonProperty]
		public Date date;
	}
}
