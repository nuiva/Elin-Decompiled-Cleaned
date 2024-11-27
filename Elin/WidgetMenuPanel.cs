using System;
using UnityEngine;
using UnityEngine.UI;

public class WidgetMenuPanel : Widget, IChangeResolution
{
	public override object CreateExtra()
	{
		return new WidgetMenuPanel.Extra();
	}

	public WidgetMenuPanel.Extra extra
	{
		get
		{
			return base.config.extra as WidgetMenuPanel.Extra;
		}
	}

	private bool allMenu
	{
		get
		{
			return EMono.debug.allMenu;
		}
	}

	public override void OnActivate()
	{
		WidgetMenuPanel.Instance = this;
		this.mold.skinRoot = base.GetComponent<SkinRoot>();
		this.Build();
	}

	public static void OnChangeMode()
	{
		if (WidgetMenuPanel.Instance)
		{
			WidgetMenuPanel.Instance._OnChangeMode();
		}
	}

	public void _OnChangeMode()
	{
		if (!WidgetMenuPanel.Instance || WidgetMenuPanel.Instance == null)
		{
			return;
		}
		this.buttonBuild.SetActive(EMono._zone.CanEnterBuildModeAnywhere);
		int num = ((EMono._zone.mainFaction == EMono.pc.faction) ? 2 : 0) + (EMono._zone.CanEnterBuildModeAnywhere ? 1 : 0);
		this.imageGrid.uvRect = new Rect(1f, 1f, (float)num, 1f);
	}

	public override void OnChangeResolution()
	{
		base.OnChangeResolution();
	}

	public void FixSize()
	{
	}

	public void Build()
	{
		this.layout.DestroyChildren(false, true);
		this.layout2.DestroyChildren(false, true);
		this.buttonBuild = this.AddButton(this.layout, "Inspect", delegate
		{
			if (EMono.ui.BlockInput)
			{
				SE.BeepSmall();
				return;
			}
			EMono.player.hotbars.ResetHotbar(3);
			EMono.player.hotbars.bars[3].dirty = true;
			EMono.player.hotbars.ResetHotbar(4);
			EMono.player.hotbars.bars[4].dirty = true;
			ActionMode.Inspect.Activate(true, false);
		}, false);
		this._OnChangeMode();
		this.RebuildLayout(true);
	}

	public UIButton AddButton(LayoutGroup _layout, string id, Action action, bool enableSubtext = false)
	{
		UIButton uibutton = Util.Instantiate<UIButton>(this.mold, _layout);
		uibutton.icon.sprite = SpriteSheet.Get("icon_" + id);
		uibutton.subText.SetActive(enableSubtext);
		uibutton.tooltip.text = id.lang();
		if (action != null)
		{
			uibutton.onClick.AddListener(delegate()
			{
				action();
			});
		}
		return uibutton;
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uicontextMenu = m.AddChild("style");
		uicontextMenu.AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", (float)base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			this.ApplySkin();
		}, 0f, (float)(base.config.skin.Skin.buttons.Count - 1), true, true, false);
		base.SetGridContextMenu(uicontextMenu);
		base.SetBaseContextMenu(m);
	}

	public static WidgetMenuPanel Instance;

	public LayoutGroup layout;

	public LayoutGroup layout2;

	public UIButton mold;

	public RawImage imageGrid;

	public int maxWidth;

	public int marginLeft;

	public int marginRight;

	[NonSerialized]
	public UIButton buttonHome;

	[NonSerialized]
	public UIButton buttonBuild;

	[NonSerialized]
	public UIButton buttonPeople;

	[NonSerialized]
	public UIButton buttonGlobalMap;

	public class Extra
	{
	}
}
