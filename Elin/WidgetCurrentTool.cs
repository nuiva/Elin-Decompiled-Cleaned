using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WidgetCurrentTool : Widget
{
	public override object CreateExtra()
	{
		return new WidgetCurrentTool.Extra();
	}

	public WidgetCurrentTool.Extra extra
	{
		get
		{
			return base.config.extra as WidgetCurrentTool.Extra;
		}
	}

	public int page
	{
		get
		{
			return EMono.player.hotbarPage;
		}
		set
		{
			EMono.player.hotbarPage = value;
		}
	}

	public override void OnActivate()
	{
		this.transHighlightSwitch.SetActive(!EMono.player.flags.toggleHotbarHighlightDisabled);
		WidgetCurrentTool.Instance = this;
		this._RefreshCurrentHotItem();
		this.buttonHotItem.onClick.AddListener(new UnityAction(this.OnClickButtonHotItem));
		this.buttonSwitchPage.SetOnClick(delegate
		{
			if (!EMono.player.flags.toggleHotbarHighlightDisabled && this.page == 1)
			{
				EMono.player.flags.toggleHotbarHighlightDisabled = true;
				this.transHighlightSwitch.SetActive(false);
			}
			this.SwitchPage();
		});
		this.RebuildSlots();
		WidgetCurrentTool.dirty = true;
		this.placer.Refresh();
		this.ApplySkin();
	}

	public void RebuildSlots()
	{
		this.imageHighlight.transform.SetParent(base.transform, false);
		this.layout.cellSize = EMono.setting.ui.iconSizes[this.extra.iconSize];
		SkinConfig skin = base.config.skin;
		this.list.Clear();
		this.list.callbacks = new UIList.Callback<Thing, ButtonGridDrag>
		{
			onRedraw = delegate(Thing a, ButtonGridDrag b, int i)
			{
				int num = i + 1;
				b.subText2.SetText((num % 10).ToString() ?? "");
				b.subText2.SetActive(this.extra.showShortcut);
				b.index = i % 10 + 10 * this.page;
				b.SetCardGrid(a, new InvOwnerHotbar(EMono.pc, null, CurrencyType.None)
				{
					index = i % 10 + 10 * this.page
				});
			}
		};
		for (int j = 0; j < this.maxSlots; j++)
		{
			this.list.Add(null);
		}
		this.list.onBeforeRedraw = delegate()
		{
			WidgetCurrentTool.dirty = false;
			for (int i = 0; i < this.grid.Count; i++)
			{
				this.grid[i] = null;
			}
			foreach (Thing thing in EMono.pc.things)
			{
				if (thing.invY == 1 && thing.invX >= this.page * 10 && thing.invX < (this.page + 1) * 10)
				{
					if (thing.invX < 0 || thing.invX >= this.grid.Count)
					{
						Debug.Log(thing.Name + "/" + thing.invX.ToString());
					}
					else
					{
						this.grid[thing.invX] = thing;
					}
				}
			}
			for (int k = 0; k < this.list.buttons.Count; k++)
			{
				UIList.ButtonPair value = this.list.buttons[k];
				value.obj = this.grid[k + this.page * 10];
				this.list.buttons[k] = value;
			}
			this.buttonSwitchPage.mainText.text = ((this.page + 1).ToString() ?? "");
		};
		this.list.onAfterRedraw = delegate()
		{
			LayerInventory.TryShowGuide(this.list);
		};
		this.list.Refresh(false);
		this.list.Redraw();
		UIRawImage uirawImage = this.list.bgGrid as UIRawImage;
		uirawImage.skinRoot = base.GetComponent<SkinRoot>();
		uirawImage.color = skin.gridColor;
		base.ClampToScreen();
		this.RefreshHighlight();
	}

	private void Update()
	{
		this.CheckDirty();
	}

	public void CheckDirty()
	{
		if (WidgetCurrentTool.dirty)
		{
			this.list.Redraw();
			this._RefreshCurrentHotItem();
		}
		WidgetCurrentTool.dirty = false;
	}

	public static void RefreshCurrentHotItem()
	{
		if (WidgetCurrentTool.Instance)
		{
			WidgetCurrentTool.Instance._RefreshCurrentHotItem();
		}
	}

	public void SwitchPage()
	{
		this.page = ((this.page == 0) ? 1 : 0);
		this._RefreshCurrentHotItem();
		WidgetCurrentTool.dirty = true;
		this.CheckDirty();
		this.placer.Refresh();
		SE.Play("switch_hotbar");
	}

	public void _RefreshCurrentHotItem()
	{
		HotItem hotItem = EMono.player.currentHotItem;
		if (hotItem == null)
		{
			hotItem = EMono.player.hotItemNoItem;
		}
		this.buttonHotItem.item = hotItem;
		this.buttonHotItem.icon.material = (hotItem.UseUIObjMaterial ? EMono.core.refs.matUIObj : null);
		hotItem.SetImage(this.buttonHotItem.icon);
		this.buttonHotItem.interactable = true;
		this.buttonHotItem.Refresh();
		this.iconHeld.SetActive(hotItem is HotItemHeld);
		this.RefreshHighlight();
	}

	public void OnClickButtonHotItem()
	{
		SE.SelectHotitem();
		if (this.selected != -1 && EMono.player.currentHotItem.Thing == this.selectedButton.card)
		{
			this.Select(-1, false);
			return;
		}
		int index = this.selected;
		this.selected = -1;
		this.Select(index, false);
	}

	public ButtonGrid selectedButton
	{
		get
		{
			return this.list.buttons[this.selected].component as ButtonGrid;
		}
	}

	public void ModSelected(int a)
	{
		SE.SelectHotitem();
		int index = (a > 0) ? this.GetNextSelectableIndex() : this.GetPrevSelectableIndex();
		this.Select(index, false);
	}

	public int GetNextSelectableIndex()
	{
		int num = this.selected + 1;
		if (num >= this.maxSlots)
		{
			num = -1;
		}
		return num;
	}

	public int GetPrevSelectableIndex()
	{
		int num = this.selected - 1;
		if (num <= -2)
		{
			num = this.maxSlots - 1;
		}
		return num;
	}

	public void Reselect()
	{
		this.Select(this.selected, false);
	}

	public void Select(int index, bool fromHotkey = false)
	{
		if (index != -1 && fromHotkey && EMono.core.config.game.useAbilityOnHotkey)
		{
			ButtonGrid buttonGrid = this.list.buttons[index].component as ButtonGrid;
			TraitAbility traitAbility = (buttonGrid.card != null) ? (buttonGrid.card.trait as TraitAbility) : null;
			if (traitAbility != null && traitAbility.CanUse(EMono.pc))
			{
				if (EMono.player.CanAcceptInput() && traitAbility.OnUse(EMono.pc))
				{
					EMono.player.EndTurn(true);
				}
				return;
			}
		}
		if (fromHotkey)
		{
			SE.SelectHotitem();
		}
		if (this.selected == index && this.selectedPage == this.page)
		{
			index = -1;
		}
		this.selected = index;
		this.selectedPage = this.page;
		if (this.selected >= this.maxSlots)
		{
			this.selected = -1;
		}
		if (this.selected == -1 || this.selectedButton.card == null)
		{
			EMono.player.ResetCurrentHotItem();
		}
		else
		{
			EMono.player.SetCurrentHotItem(this.selectedButton.card.trait.GetHotItem());
		}
		if (ActionMode.Adv.IsActive)
		{
			ActionMode.Adv.UpdatePlans();
		}
		this.RefreshHighlight();
	}

	public void RefreshHighlight()
	{
		bool flag = this.selected != -1;
		this.imageHighlight.SetActive(flag);
		if (flag)
		{
			this.imageHighlight.transform.SetParent(this.selectedButton.transform, false);
			this.imageHighlight.transform.position = this.selectedButton.transform.position;
			this.imageHighlight.Rect().sizeDelta = this.selectedButton.Rect().sizeDelta;
			this.imageHighlight.transform.SetAsFirstSibling();
			this.imageHighlight.SetAlpha((EMono.player.currentHotItem.Thing == this.selectedButton.card) ? 1f : 0.35f);
		}
	}

	public override bool CanShowContextMenu()
	{
		ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
		return (!componentOf || !(componentOf != this.buttonHotItem)) && base.CanShowContextMenu();
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		InputModuleEX.GetComponentOf<ButtonHotItem>();
		UIContextMenu uicontextMenu = m.AddChild("setting");
		UIContextMenu uicontextMenu2 = m.AddChild("style");
		uicontextMenu2.AddSlider("iconSize", (float n) => n.ToString() ?? "", (float)this.extra.iconSize, delegate(float a)
		{
			this.extra.iconSize = (int)a;
			this.RebuildSlots();
		}, 0f, (float)(EMono.setting.ui.iconSizes.Count - 1), true, true, false);
		base.SetGridContextMenu(uicontextMenu2);
		uicontextMenu.AddToggle("showShortcut", this.extra.showShortcut, delegate(bool a)
		{
			this.extra.showShortcut = a;
			this.RebuildSlots();
		});
		base.SetBaseContextMenu(m);
	}

	public const int SlotsPerPage = 10;

	public const int MaxPage = 2;

	public static WidgetCurrentTool Instance;

	public ButtonHotItem buttonHotItem;

	public UIButton buttonSwitchPage;

	public Image iconHeld;

	public Image imageHighlight;

	public UIList list;

	public static bool dirty;

	public int maxSlots = 9;

	public List<Thing> grid = new List<Thing>(new Thing[20]);

	public UIPlaceHelper placer;

	public GridLayoutGroup layout;

	public Transform transHighlightSwitch;

	public int selected = -1;

	public int selectedPage;

	public class Extra
	{
		public int iconSize;

		public bool showShortcut;

		public bool top;
	}
}
