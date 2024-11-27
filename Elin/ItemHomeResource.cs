using System;
using UnityEngine.UI;

public class ItemHomeResource : EMono
{
	public void SetResource(BaseHomeResource _r)
	{
		BaseHomeResource.ResourceGroup group = _r.Group;
		if (group != BaseHomeResource.ResourceGroup.Currency)
		{
			if (group != BaseHomeResource.ResourceGroup.Rate)
			{
				this.button.mainText.text = "Lv." + _r.value.ToString();
				this.button.subText.SetActive(false);
			}
			else
			{
				HomeResourceRate homeResourceRate = _r as HomeResourceRate;
				int num = homeResourceRate.value - homeResourceRate.lastValue;
				if (num != 0)
				{
				}
				string str = "";
				if (num > 0)
				{
					str = "↑".TagColor(FontColor.Good, null);
				}
				else if (num < 0)
				{
					str = "↓".TagColor(FontColor.Bad, null);
				}
				string text = homeResourceRate.value.ToString() + " " + str;
				this.button.mainText.text = text;
				this.button.subText.SetActive(false);
			}
		}
		else
		{
			HomeResource homeResource = _r as HomeResource;
			int num2 = homeResource.value - homeResource.lastValue;
			int num3 = (num2 == 0) ? 4 : ((num2 > 0) ? 5 : 6);
			string text2 = homeResource.value.ToString() ?? "";
			if (homeResource.value < 0)
			{
				text2 = text2.TagColor(FontColor.Bad, null);
			}
			string text3 = text2;
			this.button.mainText.text = text3;
			FontColor c = (num2 == 0) ? FontColor.Passive : ((num2 > 0) ? FontColor.Good : FontColor.Bad);
			this.button.subText.text = ((((num2 > 0) ? "+" : "") + num2.ToString()).TagColor(c, null) + "dailyIncome".lang()).TagSize(13);
		}
		this.imageExp.SetActive(_r.IsSkill);
		this.imageExp.fillAmount = _r.ExpRatio;
		this.button.icon.sprite = _r.Sprite;
		this.button.tooltip.onShowTooltip = delegate(UITooltip tp)
		{
			_r.WriteNote(tp.note);
		};
	}

	public UIButton button;

	public Image imageExp;
}
