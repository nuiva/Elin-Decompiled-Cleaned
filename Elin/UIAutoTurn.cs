using UnityEngine;
using UnityEngine.UI;

public class UIAutoTurn : EMono
{
	public Image icon;

	public Sprite[] iconSprites;

	public Gauge gauge;

	public string hint;

	private int count;

	private void Awake()
	{
		InvokeRepeating("_Update", 0f, 0.05f);
	}

	private void _Update()
	{
		float now = 0f;
		float num = 0f;
		hint = null;
		if (EMono.core.IsGameStarted)
		{
			if (EMono.pc.WillConsumeTurn())
			{
				num = 1f;
				for (int num2 = EMono.pc.conditions.Count - 1; num2 >= 0; num2--)
				{
					if (EMono.pc.conditions[num2].ConsumeTurn)
					{
						hint = EMono.pc.conditions[num2].GetText().ToTitleCase();
					}
				}
			}
			else if (!EMono.pc.HasNoGoal)
			{
				AIProgress progress = EMono.pc.ai.GetProgress();
				if (EMono.pc.ai.IsAutoTurn || progress != null)
				{
					if (progress != null)
					{
						hint = progress.TextHint;
						num = (progress.ShowProgress ? progress.MaxProgress : 0);
						now = (progress.ShowProgress ? progress.progress : 0);
					}
					else
					{
						num = EMono.pc.ai.MaxProgress;
						now = (EMono.pc.ai.ShowProgress ? EMono.pc.ai.CurrentProgress : 0);
					}
				}
			}
		}
		if (!gauge.gameObject.activeSelf && num != 0f)
		{
			gauge.bar.rectTransform.sizeDelta = new Vector2(0f, gauge.bar.rectTransform.sizeDelta.y);
		}
		gauge.SetActive(num != 0f);
		if (num != 0f)
		{
			gauge.textNow.SetActive(hint != null);
			count++;
			gauge.UpdateValue(now, num);
			if (hint != null)
			{
				gauge.textNow.SetText(hint);
			}
			icon.sprite = iconSprites[count / 5 % 2];
		}
	}
}
