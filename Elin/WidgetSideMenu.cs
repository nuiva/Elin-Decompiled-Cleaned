using System;
using UnityEngine;

public class WidgetSideMenu : Widget
{
	public override void OnActivate()
	{
		this.ChangeMode(0);
	}

	public void ChangeMode(int i)
	{
		this.ChangeMode(i.ToEnum<WidgetSideMenu.Mode>());
	}

	public void ChangeMode(WidgetSideMenu.Mode m)
	{
		this.mode = m;
		this.track.SetActive(this.mode == WidgetSideMenu.Mode.Stock);
		this.goMob.SetActive(this.mode == WidgetSideMenu.Mode.Mob);
		this.goExp.SetActive(this.mode == WidgetSideMenu.Mode.Exp);
		if (m == WidgetSideMenu.Mode.Stock)
		{
			this.track.OnActivate();
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("style").AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", (float)base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			this.ApplySkin();
		}, 0f, (float)(base.config.skin.Skin.buttons.Count - 1), true, true, false);
		base.SetBaseContextMenu(m);
	}

	public WidgetSideMenu.Mode mode;

	public GameObject goMob;

	public GameObject goExp;

	public UIResourceTrack track;

	public enum Mode
	{
		Stock,
		Mob,
		Exp
	}
}
