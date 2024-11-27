using System;
using UnityEngine;
using UnityEngine.UI;

public class WidgetSystemIndicator : Widget
{
	public static void Refresh()
	{
		if (!WidgetSystemIndicator.Instance)
		{
			return;
		}
		WidgetSystemIndicator.Instance._Refresh();
	}

	public override void OnActivate()
	{
		WidgetSystemIndicator.Instance = this;
		this._Refresh();
	}

	public void _Refresh()
	{
		this.goMuteBGM.SetActive(EMono.Sound.muteBGM);
		this.goHideBalloon.SetActive(!EMono.ui.rectDynamic.gameObject.activeSelf);
		this.goLowBlock.SetActive(EMono.game.config.showRoof);
		this.textDebug.SetActive(EMono.debug.enable);
		this.textDebug.text = "*Debug* " + EMono.core.version.GetText();
		this.RebuildLayout(true);
	}

	public override void OnFlip()
	{
		base.GetComponent<HorizontalLayoutGroup>().childAlignment = (this.flip ? TextAnchor.LowerRight : TextAnchor.LowerLeft);
		this.Rect().pivot = new Vector2((float)(this.flip ? 1 : 0), 0f);
	}

	public static WidgetSystemIndicator Instance;

	public GameObject goMuteBGM;

	public GameObject goHideBalloon;

	public GameObject goLowBlock;

	public UIText textDebug;
}
