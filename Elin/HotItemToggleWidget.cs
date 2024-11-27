using System;

public class HotItemToggleWidget : HotItemIcon
{
	public override int defaultIcon
	{
		get
		{
			return 11;
		}
	}

	public override string Name
	{
		get
		{
			return "s_toggleWidgets".lang();
		}
	}

	public override void OnClick(UIButton b)
	{
		EClass.ui.widgets.UpdateConfigs();
		EClass.ui.widgets.Reset(true);
		SE.Tab();
	}
}
