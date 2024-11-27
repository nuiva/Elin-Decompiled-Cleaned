using System;
using UnityEngine;

public class WidgetExpHome : Widget
{
	public override object CreateExtra()
	{
		return new WidgetExpHome.Extra();
	}

	public WidgetExpHome.Extra extra
	{
		get
		{
			return base.config.extra as WidgetExpHome.Extra;
		}
	}

	public override void OnActivate()
	{
		this.Build();
		base.InvokeRepeating("Refresh", 0.5f, 0.5f);
	}

	public void Build()
	{
		this.RebuildLayout(false);
		this.Refresh();
	}

	public void OnClick()
	{
		if (EMono.ui.BlockInput)
		{
			SE.BeepSmall();
			return;
		}
		LayerChara layerChara = EMono.ui.ToggleLayer<LayerChara>(null);
		if (layerChara)
		{
			layerChara.SetChara(EMono.pc).windows[0].SwitchContent(2);
			layerChara.windowChara.ToggleFeatMode();
		}
	}

	public void Refresh()
	{
		this.goLv.SetActive(EMono.pc.feat > 0);
		this.textLv.text = (EMono.pc.feat.ToString() ?? "");
		this.itemExp.text1.text = EMono.pc.exp.ToString() + "/" + EMono.pc.ExpToNext.ToString();
		this.itemExp.image1.fillAmount = (float)EMono.pc.exp / (float)EMono.pc.ExpToNext + 0.01f;
	}

	public GameObject goLv;

	public UIText textLv;

	public UIItem itemExp;

	public UIItem itemKnowledge;

	public UIItem itemInfluence;

	public class Extra
	{
		public bool lv;

		public bool knowledge;

		public bool influence;
	}
}
