using UnityEngine;

public class WidgetExp : Widget
{
	public class Extra
	{
		public bool lv;

		public bool knowledge;

		public bool influence;
	}

	public GameObject goLv;

	public UIText textLv;

	public UIItem itemExp;

	public UIItem itemKnowledge;

	public UIItem itemInfluence;

	public Extra extra => base.config.extra as Extra;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		Build();
		InvokeRepeating("Refresh", 0.5f, 0.5f);
	}

	public void Build()
	{
		this.RebuildLayout();
		Refresh();
	}

	public void OnClick()
	{
		if (EMono.ui.BlockInput)
		{
			SE.BeepSmall();
			return;
		}
		LayerChara layerChara = EMono.ui.ToggleLayer<LayerChara>();
		if ((bool)layerChara)
		{
			layerChara.SetChara(EMono.pc).windows[0].SwitchContent(2);
			layerChara.windowChara.ToggleFeatMode();
		}
	}

	public void Refresh()
	{
		goLv.SetActive(EMono.pc.feat > 0);
		textLv.text = EMono.pc.feat.ToString() ?? "";
		itemExp.text1.text = EMono.pc.exp + "/" + EMono.pc.ExpToNext;
		itemExp.image1.fillAmount = (float)EMono.pc.exp / (float)EMono.pc.ExpToNext + 0.01f;
	}
}
