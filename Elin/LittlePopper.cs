public class LittlePopper : EMono
{
	public static LittlePopper Instance;

	public static bool skipPop;

	public static bool showStock;

	public PopManager pop;

	private void Awake()
	{
		Instance = this;
	}

	public static void OnAddStockpile(Thing t, int num)
	{
		if ((bool)Instance && showStock && !skipPop)
		{
			Instance._OnAddStockpile(t, num);
		}
	}

	public void _OnAddStockpile(Thing t, int num)
	{
		pop.PopText(t.NameSimple + " (" + Lang._ChangeNum(t.Num - num, t.Num) + ")", null, "PopStock");
	}
}
