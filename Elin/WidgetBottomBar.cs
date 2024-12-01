using System;
using UnityEngine;
using UnityEngine.UI;

public class WidgetBottomBar : Widget
{
	public class Extra
	{
		public int width;

		public int height;

		public bool subBar;
	}

	public Image imageSubBar;

	public override bool AlwaysBottom => true;

	public override Type SetSiblingAfter => typeof(WidgetSideScreen);

	public Extra extra => base.config.extra as Extra;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		Refresh();
		OnChangeResolution();
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		_ = base.config.skin;
		UIContextMenu uIContextMenu = m.AddChild("setting");
		uIContextMenu.AddSlider("width", (float a) => a.ToString() ?? "", extra.width, delegate(float a)
		{
			extra.width = (int)a;
			OnChangeResolution();
		}, 10f, 100f, isInt: true);
		uIContextMenu.AddSlider("height", (float a) => a.ToString() ?? "", extra.height, delegate(float a)
		{
			extra.height = (int)a;
			OnChangeResolution();
		}, 5f, 300f, isInt: true);
		uIContextMenu.AddToggle("subBar", extra.subBar, delegate
		{
			extra.subBar = !extra.subBar;
			Refresh();
		});
		SetBaseContextMenu(m);
	}

	public void Refresh()
	{
		imageSubBar.SetActive(extra.subBar);
	}

	public override void OnChangeResolution()
	{
		base.OnChangeResolution();
		this.Rect().sizeDelta = new Vector2(0.01f * (float)Screen.width * (float)extra.width / EMono.core.uiScale + 2f, extra.height + 10);
	}
}
