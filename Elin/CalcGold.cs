public class CalcGold : EClass
{
	public static int ExpandLand()
	{
		return 4 + EClass._map.bounds.Width / 5;
	}

	public static int Hire(Chara c)
	{
		return c.GetHireCost();
	}
}
