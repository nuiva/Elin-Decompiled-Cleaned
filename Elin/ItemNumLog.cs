using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ItemNumLog : EMono
{
	public void SetLog(NumLog log)
	{
		this.textTitle.text = log.Name;
		this.textCurrent.text = (log.Value.ToString() ?? "");
		this.textLastDay.text = (log.lastDay.ToString() ?? "");
		this.textLastMonth.text = (log.lastMonth.ToString() ?? "");
		this.textLastYear.text = (log.lastYear.ToString() ?? "");
		if (this.layout)
		{
			UIItem mold = this.layout.CreateMold(null);
			List<Gross.Mod> mods = log.gross.GetMods();
			Action<string> action = delegate(string a)
			{
				Util.Instantiate<UIItem>(mold, this.layout).text1.text = a;
			};
			if (mods.Count == 0)
			{
				action(Lang.Get("noMod"));
				return;
			}
			foreach (Gross.Mod mod in mods)
			{
				action("");
			}
		}
	}

	public UIText textTitle;

	public UIText textCurrent;

	public UIText textLastDay;

	public UIText textLastMonth;

	public UIText textLastYear;

	public LayoutGroup layout;
}
