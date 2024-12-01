using System.Collections.Generic;

public class TimeTable : EClass
{
	public enum Span
	{
		Free,
		Eat,
		Work,
		Sleep
	}

	public static Dictionary<string, TimeTable> dict = new Dictionary<string, TimeTable>();

	private static bool unityInit;

	public Span[] spans = new Span[24];

	public static void Init()
	{
		if (!unityInit)
		{
			Add("zzzzzz  ewwwwwwwwwwe    z", "default");
			Add("zzzzzz  ewwwwwwwwwwe    z", "owl");
			unityInit = true;
		}
	}

	public static TimeTable Add(string raw, string id)
	{
		TimeTable timeTable = new TimeTable();
		for (int i = 0; i < 24; i++)
		{
			timeTable.spans[i] = GetSpan(raw[i]);
		}
		dict.Add(id, timeTable);
		return timeTable;
	}

	public static TimeTable GetTimeTable(string id)
	{
		return dict[id];
	}

	public static Span GetSpan(string id, int hour)
	{
		return dict[id].spans[hour];
	}

	private static Span GetSpan(char s)
	{
		return s switch
		{
			'z' => Span.Sleep, 
			'e' => Span.Eat, 
			'w' => Span.Work, 
			_ => Span.Free, 
		};
	}

	public Span GetSpan(int hour)
	{
		return Span.Free;
	}
}
