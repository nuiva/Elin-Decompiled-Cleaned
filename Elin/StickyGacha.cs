public class StickyGacha : BaseSticky
{
	public override int idIcon => 2;

	public override string idLang => "sticky_gacha";

	public new WidgetSticky widget => WidgetSticky.Instance;

	public override bool ShouldShow
	{
		get
		{
			if (!EClass.ui.GetLayer<LayerGacha>())
			{
				return EClass.player.dailyGacha;
			}
			return false;
		}
	}

	public override void OnClick()
	{
		EClass.ui.AddLayer<LayerGacha>();
		widget.Refresh();
	}
}
