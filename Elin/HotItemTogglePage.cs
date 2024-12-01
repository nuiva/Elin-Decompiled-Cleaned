public class HotItemTogglePage : HotItem
{
	public override string Name => "s_page".lang();

	public override string TextTip => null;

	public override string pathSprite => "icon_page";

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
