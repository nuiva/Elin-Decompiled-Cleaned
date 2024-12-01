public class DayData
{
	public enum Luck
	{
		Best,
		Great,
		Good,
		Average,
		Bad,
		Awful
	}

	public static int[] LuckRange = new int[6] { 95, 80, 60, 30, 10, 0 };

	public Luck luck = Luck.Awful;

	public int seed;
}
