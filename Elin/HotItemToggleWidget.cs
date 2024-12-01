public class HotItemToggleWidget : HotItemIcon
{
	public override int defaultIcon => 11;

	public override string Name => "s_toggleWidgets".lang();

	public override void OnClick(UIButton b)
	{
		EClass.ui.widgets.UpdateConfigs();
		EClass.ui.widgets.Reset(toggleTheme: true);
		SE.Tab();
	}
}
