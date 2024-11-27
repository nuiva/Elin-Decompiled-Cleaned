using System;
using UnityEngine;
using UnityEngine.UI;

public class WidgetBottomBar : Widget
{
	public override object CreateExtra()
	{
		return new WidgetBottomBar.Extra();
	}

	public override bool AlwaysBottom
	{
		get
		{
			return true;
		}
	}

	public override Type SetSiblingAfter
	{
		get
		{
			return typeof(WidgetSideScreen);
		}
	}

	public WidgetBottomBar.Extra extra
	{
		get
		{
			return base.config.extra as WidgetBottomBar.Extra;
		}
	}

	public override void OnActivate()
	{
		this.Refresh();
		this.OnChangeResolution();
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		SkinConfig skin = base.config.skin;
		UIContextMenu uicontextMenu = m.AddChild("setting");
		uicontextMenu.AddSlider("width", (float a) => a.ToString() ?? "", (float)this.extra.width, delegate(float a)
		{
			this.extra.width = (int)a;
			this.OnChangeResolution();
		}, 10f, 100f, true, true, false);
		uicontextMenu.AddSlider("height", (float a) => a.ToString() ?? "", (float)this.extra.height, delegate(float a)
		{
			this.extra.height = (int)a;
			this.OnChangeResolution();
		}, 5f, 300f, true, true, false);
		uicontextMenu.AddToggle("subBar", this.extra.subBar, delegate(bool a)
		{
			this.extra.subBar = !this.extra.subBar;
			this.Refresh();
		});
		base.SetBaseContextMenu(m);
	}

	public void Refresh()
	{
		this.imageSubBar.SetActive(this.extra.subBar);
	}

	public override void OnChangeResolution()
	{
		base.OnChangeResolution();
		this.Rect().sizeDelta = new Vector2(0.01f * (float)Screen.width * (float)this.extra.width / EMono.core.uiScale + 2f, (float)(this.extra.height + 10));
	}

	public Image imageSubBar;

	public class Extra
	{
		public int width;

		public int height;

		public bool subBar;
	}
}
