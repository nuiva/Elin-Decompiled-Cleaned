using System;

public class NotificationHome : NotificationGlobal
{
	public override void OnClick()
	{
		EClass.ui.AddLayer<LayerHome>();
	}

	public override void OnRefresh()
	{
		this.text = "1.".TagSize("00", this.item.button.mainText.fontSize - 2);
	}

	public override Action<UITooltip> onShowTooltip
	{
		get
		{
			return delegate(UITooltip a)
			{
				a.textMain.text = "factionRank".lang() + ": " + EClass.Branch.RankText;
			};
		}
	}
}
