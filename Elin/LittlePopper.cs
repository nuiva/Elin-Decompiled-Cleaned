using System;
using UnityEngine;

public class LittlePopper : EMono
{
	private void Awake()
	{
		LittlePopper.Instance = this;
	}

	public static void OnAddStockpile(Thing t, int num)
	{
		if (LittlePopper.Instance && LittlePopper.showStock && !LittlePopper.skipPop)
		{
			LittlePopper.Instance._OnAddStockpile(t, num);
		}
	}

	public void _OnAddStockpile(Thing t, int num)
	{
		this.pop.PopText(t.NameSimple + " (" + Lang._ChangeNum(t.Num - num, t.Num) + ")", null, "PopStock", default(Color), default(Vector3), 0f);
	}

	public static LittlePopper Instance;

	public static bool skipPop;

	public static bool showStock;

	public PopManager pop;
}
