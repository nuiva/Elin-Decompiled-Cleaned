using System;
using UnityEngine;
using UnityEngine.UI;

public class UIAutoTurn : EMono
{
	private void Awake()
	{
		base.InvokeRepeating("_Update", 0f, 0.05f);
	}

	private void _Update()
	{
		float now = 0f;
		float num = 0f;
		this.hint = null;
		if (EMono.core.IsGameStarted)
		{
			if (EMono.pc.WillConsumeTurn())
			{
				num = 1f;
				for (int i = EMono.pc.conditions.Count - 1; i >= 0; i--)
				{
					if (EMono.pc.conditions[i].ConsumeTurn)
					{
						this.hint = EMono.pc.conditions[i].GetText().ToTitleCase(false);
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
						this.hint = progress.TextHint;
						num = (float)(progress.ShowProgress ? progress.MaxProgress : 0);
						now = (float)(progress.ShowProgress ? progress.progress : 0);
					}
					else
					{
						num = (float)EMono.pc.ai.MaxProgress;
						now = (float)(EMono.pc.ai.ShowProgress ? EMono.pc.ai.CurrentProgress : 0);
					}
				}
			}
		}
		if (!this.gauge.gameObject.activeSelf & num != 0f)
		{
			this.gauge.bar.rectTransform.sizeDelta = new Vector2(0f, this.gauge.bar.rectTransform.sizeDelta.y);
		}
		this.gauge.SetActive(num != 0f);
		if (num == 0f)
		{
			return;
		}
		this.gauge.textNow.SetActive(this.hint != null);
		this.count++;
		this.gauge.UpdateValue(now, num);
		if (this.hint != null)
		{
			this.gauge.textNow.SetText(this.hint);
		}
		this.icon.sprite = this.iconSprites[this.count / 5 % 2];
	}

	public Image icon;

	public Sprite[] iconSprites;

	public Gauge gauge;

	public string hint;

	private int count;
}
