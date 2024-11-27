using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetSticky : Widget
{
	public override object CreateExtra()
	{
		return new WidgetSticky.Extra();
	}

	public WidgetSticky.Extra extra
	{
		get
		{
			return base.config.extra as WidgetSticky.Extra;
		}
	}

	public override void OnActivate()
	{
		WidgetSticky.Instance = this;
		this.Init();
	}

	public void Init()
	{
		WidgetSticky.Instance = this;
		this.mold = this.layout.CreateMold(null);
		this._Add(new StickyMenu(), false);
		this._Add(new StickyDate(), false);
		this._Add(new StickyWelcome(), false);
		this._Add(new StickyGacha(), false);
		base.InvokeRepeating("Refresh", 0f, 1f);
	}

	public static void Add(BaseSticky sticky, bool animate = true)
	{
		if (!WidgetSticky.Instance)
		{
			return;
		}
		WidgetSticky.Instance._Add(sticky, true);
	}

	public void _Add(BaseSticky sticky, bool animate = true)
	{
		if (!sticky.AllowMultiple)
		{
			this.list.ForeachReverse(delegate(BaseSticky i)
			{
				if (i.GetType().Equals(sticky.GetType()))
				{
					this._Remove(i);
				}
			});
		}
		this.list.Add(sticky);
		UIItem uiitem = sticky.item = Util.Instantiate<UIItem>(this.mold, this.layout);
		uiitem.image1.transform.parent.SetActive(sticky.animate);
		uiitem.SetActive(true);
		sticky.RefreshButton();
		uiitem.transform.SetAsFirstSibling();
		this.layout.RebuildLayout(false);
		if (animate)
		{
			this.anime.Play(uiitem.button1.transform, null, -1f, 0f);
			SE.Play(sticky.idSound);
		}
	}

	public void _Remove(BaseSticky sticky)
	{
		this.list.Remove(sticky);
		UnityEngine.Object.Destroy(sticky.item.gameObject);
	}

	public void Refresh()
	{
		foreach (BaseSticky baseSticky in this.list)
		{
			bool shouldShow = baseSticky.ShouldShow;
			baseSticky.item.SetActive(shouldShow);
			if (shouldShow)
			{
				baseSticky.Refresh();
			}
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uicontextMenu = m.AddChild("setting");
		uicontextMenu.AddToggle("showDate", this.extra.showDate, delegate(bool a)
		{
			this.extra.showDate = a;
			this.Refresh();
		});
		uicontextMenu.AddToggle("showText", this.extra.showText, delegate(bool a)
		{
			this.extra.showText = a;
			this.Refresh();
		});
		uicontextMenu.AddToggle("showNerun", this.extra.showNerun, delegate(bool a)
		{
			this.extra.showNerun = a;
			this.Refresh();
		});
		m.AddChild("style").AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", (float)base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			this.ApplySkin();
		}, 0f, (float)(base.config.skin.Skin.buttons.Count - 1), true, true, false);
		base.SetBaseContextMenu(m);
	}

	public static WidgetSticky Instance;

	public LayoutGroup layout;

	public List<BaseSticky> list = new List<BaseSticky>();

	public List<Sprite> icons;

	public UIItem mold;

	public Anime anime;

	public class Extra
	{
		public bool showDate;

		public bool showText;

		public bool showNerun;
	}
}
