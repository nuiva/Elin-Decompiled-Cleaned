using System;
using System.Collections.Generic;

public class NotificationBattle : NotificationGlobal
{
	public int index;

	public override bool Visible => widget.battles > 0;

	public override Action<UITooltip> onShowTooltip => delegate(UITooltip a)
	{
		a.textMain.text = "battles".lang() + ": " + widget.battles;
	};

	public override void OnRefresh()
	{
		text = widget.battles.ToString() ?? "";
	}

	public override void OnClick()
	{
		if (EClass.AdvMode)
		{
			return;
		}
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.IsHomeMember() && chara.enemy != null && chara.IsAliveInCurrentZone)
			{
				list.Add(chara);
			}
		}
		index++;
		if (index >= list.Count)
		{
			index = 0;
		}
		EClass.screen.Focus(list[index]);
	}
}
