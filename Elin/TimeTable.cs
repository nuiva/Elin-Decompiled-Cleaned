using System;
using System.Collections.Generic;

public class TimeTable : EClass
{
	public static void Init()
	{
		if (TimeTable.unityInit)
		{
			return;
		}
		TimeTable.Add("zzzzzz  ewwwwwwwwwwe    z", "default");
		TimeTable.Add("zzzzzz  ewwwwwwwwwwe    z", "owl");
		TimeTable.unityInit = true;
	}

	public static TimeTable Add(string raw, string id)
	{
		TimeTable timeTable = new TimeTable();
		for (int i = 0; i < 24; i++)
		{
			timeTable.spans[i] = TimeTable.GetSpan(raw[i]);
		}
		TimeTable.dict.Add(id, timeTable);
		return timeTable;
	}

	public static TimeTable GetTimeTable(string id)
	{
		return TimeTable.dict[id];
	}

	public static TimeTable.Span GetSpan(string id, int hour)
	{
		return TimeTable.dict[id].spans[hour];
	}

	private static TimeTable.Span GetSpan(char s)
	{
		if (s == 'e')
		{
			return TimeTable.Span.Eat;
		}
		if (s == 'w')
		{
			return TimeTable.Span.Work;
		}
		if (s == 'z')
		{
			return TimeTable.Span.Sleep;
		}
		return TimeTable.Span.Free;
	}

	public TimeTable.Span GetSpan(int hour)
	{
		return TimeTable.Span.Free;
	}

	public static Dictionary<string, TimeTable> dict = new Dictionary<string, TimeTable>();

	private static bool unityInit;

	public TimeTable.Span[] spans = new TimeTable.Span[24];

	public enum Span
	{
		Free,
		Eat,
		Work,
		Sleep
	}
}
