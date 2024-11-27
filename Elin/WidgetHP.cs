using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class WidgetHP : Widget
{
	public override object CreateExtra()
	{
		return new WidgetHP.Extra();
	}

	public WidgetHP.Extra extra
	{
		get
		{
			return base.config.extra as WidgetHP.Extra;
		}
	}

	public override void OnActivate()
	{
		this.Rebuild();
		base.InvokeRepeating("Refresh", 0f, 0.2f);
	}

	public void Rebuild()
	{
		this.grid.constraintCount = this.extra.layout + 1;
		this.Refresh();
		this.RebuildLayout(false);
	}

	public void Refresh()
	{
		if (!EMono.game.isLoading)
		{
			if (EMono.pc.hp > EMono.pc.MaxHP)
			{
				EMono.pc.hp = EMono.pc.MaxHP;
			}
			if (EMono.pc.mana.value > EMono.pc.mana.max)
			{
				EMono.pc.mana.value = EMono.pc.mana.max;
			}
			if (EMono.pc.stamina.value > EMono.pc.stamina.max)
			{
				EMono.pc.stamina.value = EMono.pc.stamina.max;
			}
		}
		this.gaugeHP.hideBar = !this.extra.showGauge;
		this.gaugeMP.hideBar = !this.extra.showGauge;
		this.gaugeStamina.hideBar = !this.extra.showGauge;
		this.gaugeStamina.SetActive(this.extra.stamina);
		this.gaugeHP.UpdateValue(EMono.pc.hp, EMono.pc.MaxHP);
		this.gaugeMP.UpdateValue(EMono.pc.mana.value, EMono.pc.mana.max);
		this.gaugeStamina.UpdateValue(EMono.pc.stamina.value, EMono.pc.stamina.max);
		Color c = EMono.Colors.Dark.gradientHP.Evaluate((float)EMono.pc.hp / (float)EMono.pc.MaxHP);
		this.gaugeHP.textNow.text = "".TagColor(c, EMono.pc.hp.ToString() ?? "") + (this.extra.showMax ? ("/" + EMono.pc.MaxHP.ToString()) : "");
		c = EMono.Colors.Dark.gradientMP.Evaluate((float)EMono.pc.mana.value / (float)EMono.pc.mana.max);
		this.gaugeMP.textNow.text = "".TagColor(c, EMono.pc.mana.value.ToString() ?? "") + (this.extra.showMax ? ("/" + EMono.pc.mana.max.ToString()) : "");
		c = EMono.Colors.Dark.gradientSP.Evaluate((float)EMono.pc.stamina.value / (float)EMono.pc.stamina.max);
		this.gaugeStamina.textNow.text = "".TagColor(c, EMono.pc.stamina.value.ToString() ?? "") + (this.extra.showMax ? ("/" + EMono.pc.stamina.max.ToString()) : "");
		this.goBarrier.SetActive(false);
		this.textBarrier.text = "10";
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uicontextMenu = m.AddChild("setting");
		uicontextMenu.AddSlider("layout", (float n) => n.ToString() ?? "", (float)this.extra.layout, delegate(float a)
		{
			this.extra.layout = (int)a;
			this.Rebuild();
			base.ClampToScreen();
		}, 0f, 2f, true, true, false);
		uicontextMenu.AddToggle("showGauge", this.extra.showGauge, delegate(bool a)
		{
			this.extra.showGauge = a;
			this.Refresh();
			this.RebuildLayout(true);
		});
		uicontextMenu.AddToggle("showMax", this.extra.showMax, delegate(bool a)
		{
			this.extra.showMax = a;
			this.Refresh();
			this.RebuildLayout(true);
		});
		uicontextMenu.AddToggle("stamina", this.extra.stamina, delegate(bool a)
		{
			this.extra.stamina = a;
			this.Refresh();
			this.RebuildLayout(true);
		});
		base.SetBaseContextMenu(m);
	}

	public Gauge gaugeHP;

	public Gauge gaugeMP;

	public Gauge gaugeStamina;

	public UIText textHP;

	public UIText textMP;

	public UIText textStamina;

	public UIText textBarrier;

	public GridLayoutGroup grid;

	public GameObject goBarrier;

	public class Extra
	{
		[JsonProperty]
		public int layout;

		[JsonProperty]
		public bool showGauge;

		[JsonProperty]
		public bool showMax;

		[JsonProperty]
		public bool stamina;
	}
}
