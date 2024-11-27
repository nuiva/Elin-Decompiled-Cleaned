using System;

public class StickyGacha : BaseSticky
{
	public override int idIcon
	{
		get
		{
			return 2;
		}
	}

	public override string idLang
	{
		get
		{
			return "sticky_gacha";
		}
	}

	public new WidgetSticky widget
	{
		get
		{
			return WidgetSticky.Instance;
		}
	}

	public override bool ShouldShow
	{
		get
		{
			return !EClass.ui.GetLayer<LayerGacha>(false) && EClass.player.dailyGacha;
		}
	}

	public override void OnClick()
	{
		EClass.ui.AddLayer<LayerGacha>();
		this.widget.Refresh();
	}
}
