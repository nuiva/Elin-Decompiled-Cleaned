using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetSticky : Widget
{
	public class Extra
	{
		public bool showDate;

		public bool showText;

		public bool showNerun;
	}

	public static WidgetSticky Instance;

	public LayoutGroup layout;

	public List<BaseSticky> list = new List<BaseSticky>();

	public List<Sprite> icons;

	public UIItem mold;

	public Anime anime;

	public Extra extra => base.config.extra as Extra;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		Instance = this;
		Init();
	}

	public void Init()
	{
		Instance = this;
		mold = layout.CreateMold<UIItem>();
		_Add(new StickyMenu(), animate: false);
		_Add(new StickyDate(), animate: false);
		_Add(new StickyWelcome(), animate: false);
		_Add(new StickyGacha(), animate: false);
		InvokeRepeating("Refresh", 0f, 1f);
	}

	public static void Add(BaseSticky sticky, bool animate = true)
	{
		if ((bool)Instance)
		{
			Instance._Add(sticky);
		}
	}

	public void _Add(BaseSticky sticky, bool animate = true)
	{
		if (!sticky.AllowMultiple)
		{
			list.ForeachReverse(delegate(BaseSticky i)
			{
				if (i.GetType().Equals(sticky.GetType()))
				{
					_Remove(i);
				}
			});
		}
		list.Add(sticky);
		UIItem uIItem = (sticky.item = Util.Instantiate(mold, layout));
		uIItem.image1.transform.parent.SetActive(sticky.animate);
		uIItem.SetActive(enable: true);
		sticky.RefreshButton();
		uIItem.transform.SetAsFirstSibling();
		layout.RebuildLayout();
		if (animate)
		{
			anime.Play(uIItem.button1.transform);
			SE.Play(sticky.idSound);
		}
	}

	public void _Remove(BaseSticky sticky)
	{
		list.Remove(sticky);
		Object.Destroy(sticky.item.gameObject);
	}

	public void Refresh()
	{
		foreach (BaseSticky item in list)
		{
			bool shouldShow = item.ShouldShow;
			item.item.SetActive(shouldShow);
			if (shouldShow)
			{
				item.Refresh();
			}
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uIContextMenu = m.AddChild("setting");
		uIContextMenu.AddToggle("showDate", extra.showDate, delegate(bool a)
		{
			extra.showDate = a;
			Refresh();
		});
		uIContextMenu.AddToggle("showText", extra.showText, delegate(bool a)
		{
			extra.showText = a;
			Refresh();
		});
		uIContextMenu.AddToggle("showNerun", extra.showNerun, delegate(bool a)
		{
			extra.showNerun = a;
			Refresh();
		});
		m.AddChild("style").AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			ApplySkin();
		}, 0f, base.config.skin.Skin.buttons.Count - 1, isInt: true);
		SetBaseContextMenu(m);
	}
}
