using UnityEngine;
using UnityEngine.UI;

public class WidgetSystemIndicator : Widget
{
	public static WidgetSystemIndicator Instance;

	public GameObject goMuteBGM;

	public GameObject goHideBalloon;

	public GameObject goLowBlock;

	public UIText textDebug;

	public static void Refresh()
	{
		if ((bool)Instance)
		{
			Instance._Refresh();
		}
	}

	public override void OnActivate()
	{
		Instance = this;
		_Refresh();
	}

	public void _Refresh()
	{
		goMuteBGM.SetActive(EMono.Sound.muteBGM);
		goHideBalloon.SetActive(!EMono.ui.rectDynamic.gameObject.activeSelf);
		goLowBlock.SetActive(EMono.game.config.showRoof);
		textDebug.SetActive(EMono.debug.enable);
		textDebug.text = "*Debug* " + EMono.core.version.GetText();
		this.RebuildLayout(recursive: true);
	}

	public override void OnFlip()
	{
		GetComponent<HorizontalLayoutGroup>().childAlignment = (flip ? TextAnchor.LowerRight : TextAnchor.LowerLeft);
		this.Rect().pivot = new Vector2(flip ? 1 : 0, 0f);
	}
}
