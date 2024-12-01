using System;
using System.Collections.Generic;

public class NotificationGuest : NotificationGlobal
{
	public int index;

	public override int idSprite => 3;

	public override bool Visible => widget.guests > 0;

	public override Action<UITooltip> onShowTooltip => delegate(UITooltip a)
	{
		a.textMain.text = "guests".lang() + ": " + widget.guests;
	};

	public override void OnRefresh()
	{
		text = widget.guests.ToString() ?? "";
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
			if (chara.IsGuest() && chara.IsAliveInCurrentZone)
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
