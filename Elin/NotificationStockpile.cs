using System;

public class NotificationStockpile : NotificationGlobal
{
	public override int idSprite
	{
		get
		{
			return 8;
		}
	}

	public override void OnRefresh()
	{
		int num = EClass._map.Stocked.weight * 100 / EClass._map.Stocked.maxWeight;
		if (num > 100)
		{
			num = 100;
		}
		this.text = num.ToString() + "%";
		this.item.button.mainText.fontColor = FontColor.DontChange;
		this.item.button.mainText.color = this.item.button.skinRoot.GetButton().colorProf.gradients["default"].Evaluate(1f - (float)num / 100f);
	}

	public override void OnClick()
	{
		EClass.ui.AddLayer<LayerResource>();
	}

	public override Action<UITooltip> onShowTooltip
	{
		get
		{
			return delegate(UITooltip a)
			{
				a.textMain.text = string.Concat(new string[]
				{
					"maxCapacity".lang(),
					": ",
					EClass._map.Stocked.weight.ToString(),
					" / ",
					EClass._map.Stocked.maxWeight.ToString()
				});
			};
		}
	}
}
