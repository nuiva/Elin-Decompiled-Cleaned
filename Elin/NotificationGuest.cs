using System;
using System.Collections.Generic;

public class NotificationGuest : NotificationGlobal
{
	public override int idSprite
	{
		get
		{
			return 3;
		}
	}

	public override bool Visible
	{
		get
		{
			return this.widget.guests > 0;
		}
	}

	public override Action<UITooltip> onShowTooltip
	{
		get
		{
			return delegate(UITooltip a)
			{
				a.textMain.text = "guests".lang() + ": " + this.widget.guests.ToString();
			};
		}
	}

	public override void OnRefresh()
	{
		this.text = (this.widget.guests.ToString() ?? "");
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
		this.index++;
		if (this.index >= list.Count)
		{
			this.index = 0;
		}
		EClass.screen.Focus(list[this.index]);
	}

	public int index;
}
