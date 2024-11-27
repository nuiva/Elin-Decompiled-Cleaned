using System;
using System.Collections.Generic;

public class NotificationBattle : NotificationGlobal
{
	public override bool Visible
	{
		get
		{
			return this.widget.battles > 0;
		}
	}

	public override Action<UITooltip> onShowTooltip
	{
		get
		{
			return delegate(UITooltip a)
			{
				a.textMain.text = "battles".lang() + ": " + this.widget.battles.ToString();
			};
		}
	}

	public override void OnRefresh()
	{
		this.text = (this.widget.battles.ToString() ?? "");
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
		this.index++;
		if (this.index >= list.Count)
		{
			this.index = 0;
		}
		EClass.screen.Focus(list[this.index]);
	}

	public int index;
}
