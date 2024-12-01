using System;
using UnityEngine;
using UnityEngine.UI;

public class WidgetMenuPanel : Widget, IChangeResolution
{
	public class Extra
	{
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

	public Extra extra => base.config.extra as Extra;

	private bool allMenu => EMono.debug.allMenu;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		Instance = this;
		mold.skinRoot = GetComponent<SkinRoot>();
		Build();
	}

	public static void OnChangeMode()
	{
		if ((bool)Instance)
		{
			Instance._OnChangeMode();
		}
	}

	public void _OnChangeMode()
	{
		if ((bool)Instance && !(Instance == null))
		{
			buttonBuild.SetActive(EMono._zone.CanEnterBuildModeAnywhere);
			int num = ((EMono._zone.mainFaction == EMono.pc.faction) ? 2 : 0) + (EMono._zone.CanEnterBuildModeAnywhere ? 1 : 0);
			imageGrid.uvRect = new Rect(1f, 1f, num, 1f);
		}
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
		layout.DestroyChildren();
		layout2.DestroyChildren();
		buttonBuild = AddButton(layout, "Inspect", delegate
		{
			if (EMono.ui.BlockInput)
			{
				SE.BeepSmall();
			}
			else
			{
				EMono.player.hotbars.ResetHotbar(3);
				EMono.player.hotbars.bars[3].dirty = true;
				EMono.player.hotbars.ResetHotbar(4);
				EMono.player.hotbars.bars[4].dirty = true;
				ActionMode.Inspect.Activate();
			}
		});
		_OnChangeMode();
		this.RebuildLayout(recursive: true);
	}

	public UIButton AddButton(LayoutGroup _layout, string id, Action action, bool enableSubtext = false)
	{
		UIButton uIButton = Util.Instantiate(mold, _layout);
		uIButton.icon.sprite = SpriteSheet.Get("icon_" + id);
		uIButton.subText.SetActive(enableSubtext);
		uIButton.tooltip.text = id.lang();
		if (action != null)
		{
			uIButton.onClick.AddListener(delegate
			{
				action();
			});
		}
		return uIButton;
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uIContextMenu = m.AddChild("style");
		uIContextMenu.AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			ApplySkin();
		}, 0f, base.config.skin.Skin.buttons.Count - 1, isInt: true);
		SetGridContextMenu(uIContextMenu);
		SetBaseContextMenu(m);
	}
}
