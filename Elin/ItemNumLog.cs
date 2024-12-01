using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ItemNumLog : EMono
{
	public UIText textTitle;

	public UIText textCurrent;

	public UIText textLastDay;

	public UIText textLastMonth;

	public UIText textLastYear;

	public LayoutGroup layout;

	public void SetLog(NumLog log)
	{
		textTitle.text = log.Name;
		textCurrent.text = log.Value.ToString() ?? "";
		textLastDay.text = log.lastDay.ToString() ?? "";
		textLastMonth.text = log.lastMonth.ToString() ?? "";
		textLastYear.text = log.lastYear.ToString() ?? "";
		if (!layout)
		{
			return;
		}
		UIItem mold = layout.CreateMold<UIItem>();
		List<Gross.Mod> mods = log.gross.GetMods();
		Action<string> action = delegate(string a)
		{
			Util.Instantiate(mold, layout).text1.text = a;
		};
		if (mods.Count == 0)
		{
			action(Lang.Get("noMod"));
			return;
		}
		foreach (Gross.Mod item in mods)
		{
			_ = item;
			action("");
		}
	}
}
