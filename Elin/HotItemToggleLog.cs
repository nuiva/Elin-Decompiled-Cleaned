using System;

public class HotItemToggleLog : HotItemIcon
{
	public override int defaultIcon
	{
		get
		{
			return 4;
		}
	}

	public override string Name
	{
		get
		{
			return "s_log".lang();
		}
	}

	public override void OnClick(UIButton b)
	{
		WidgetMainText.ToggleLog();
	}
}
