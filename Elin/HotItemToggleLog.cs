public class HotItemToggleLog : HotItemIcon
{
	public override int defaultIcon => 4;

	public override string Name => "s_log".lang();

	public override void OnClick(UIButton b)
	{
		WidgetMainText.ToggleLog();
	}
}
