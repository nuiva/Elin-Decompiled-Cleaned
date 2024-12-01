using UnityEngine.UI;

public class ItemHomeResource : EMono
{
	public UIButton button;

	public Image imageExp;

	public void SetResource(BaseHomeResource _r)
	{
		switch (_r.Group)
		{
		case BaseHomeResource.ResourceGroup.Currency:
		{
			HomeResource homeResource = _r as HomeResource;
			int num2 = homeResource.value - homeResource.lastValue;
			FontColor fontColor = ((num2 == 0) ? FontColor.Passive : ((num2 > 0) ? FontColor.Good : FontColor.Bad));
			string text3 = homeResource.value.ToString() ?? "";
			if (homeResource.value < 0)
			{
				text3 = text3.TagColor(FontColor.Bad);
			}
			string text4 = text3;
			button.mainText.text = text4;
			fontColor = ((num2 == 0) ? FontColor.Passive : ((num2 > 0) ? FontColor.Good : FontColor.Bad));
			button.subText.text = ((((num2 > 0) ? "+" : "") + num2).TagColor(fontColor) + "dailyIncome".lang()).TagSize(13);
			break;
		}
		case BaseHomeResource.ResourceGroup.Rate:
		{
			HomeResourceRate homeResourceRate = _r as HomeResourceRate;
			int num = homeResourceRate.value - homeResourceRate.lastValue;
			if (num != 0)
			{
				_ = 0;
			}
			string text = "";
			if (num > 0)
			{
				text = "↑".TagColor(FontColor.Good);
			}
			else if (num < 0)
			{
				text = "↓".TagColor(FontColor.Bad);
			}
			string text2 = homeResourceRate.value + " " + text;
			button.mainText.text = text2;
			button.subText.SetActive(enable: false);
			break;
		}
		default:
			button.mainText.text = "Lv." + _r.value;
			button.subText.SetActive(enable: false);
			break;
		}
		imageExp.SetActive(_r.IsSkill);
		imageExp.fillAmount = _r.ExpRatio;
		button.icon.sprite = _r.Sprite;
		button.tooltip.onShowTooltip = delegate(UITooltip tp)
		{
			_r.WriteNote(tp.note);
		};
	}
}
