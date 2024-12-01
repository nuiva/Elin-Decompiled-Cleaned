using System;

public class NotificationHome : NotificationGlobal
{
	public override Action<UITooltip> onShowTooltip => delegate(UITooltip a)
	{
		a.textMain.text = "factionRank".lang() + ": " + EClass.Branch.RankText;
	};

	public override void OnClick()
	{
		EClass.ui.AddLayer<LayerHome>();
	}

	public override void OnRefresh()
	{
		text = "1.".TagSize("00", item.button.mainText.fontSize - 2);
	}
}
