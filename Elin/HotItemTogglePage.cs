using System;

public class HotItemTogglePage : HotItem
{
	public override string Name
	{
		get
		{
			return "s_page".lang();
		}
	}

	public override string TextTip
	{
		get
		{
			return null;
		}
	}

	public override string pathSprite
	{
		get
		{
			return "icon_page";
		}
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		if (!b)
		{
			SE.Beep();
			return;
		}
		SE.ClickGeneral();
		b.widget.SwitchPage();
	}
}
