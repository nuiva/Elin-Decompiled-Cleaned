using System;

public class DayData
{
	public static int[] LuckRange = new int[]
	{
		95,
		80,
		60,
		30,
		10,
		0
	};

	public DayData.Luck luck = DayData.Luck.Awful;

	public int seed;

	public enum Luck
	{
		Best,
		Great,
		Good,
		Average,
		Bad,
		Awful
	}
}
