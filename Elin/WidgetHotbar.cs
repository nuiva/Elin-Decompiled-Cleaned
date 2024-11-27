using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class WidgetHotbar : Widget, IDragParent
{
	public override object CreateExtra()
	{
		return new WidgetHotbar.Extra();
	}

	public WidgetHotbar.Extra extra
	{
		get
		{
			return base.config.extra as WidgetHotbar.Extra;
		}
	}

	public Hotbar hotbar
	{
		get
		{
			return EMono.player.hotbars.bars[this.idHotbar];
		}
	}

	public bool Visible
	{
		get
		{
			return this.extra.visible;
		}
		set
		{
			this.extra.visible = value;
		}
	}

	public override bool ShowInBuildMode
	{
		get
		{
			return this.extra.alwaysShow;
		}
	}

	public bool IsHotbarSpeed
	{
		get
		{
			return this.idHotbar == 7;
		}
	}

	public bool CanRegisterItem
	{
		get
		{
			return this.hotbar.IsUserHotbar;
		}
	}

	public override bool CanShowContextMenu()
	{
		if (this.hotbar.IsUserHotbar)
		{
			ButtonHotItem componentOf = InputModuleEX.GetComponentOf<ButtonHotItem>();
			if (componentOf && componentOf.item != null)
			{
				return true;
			}
		}
		return base.CanShowContextMenu();
	}

	public override void OnActivate()
	{
		this.mold = this.layout.CreateMold(null);
		if (this.idHotbar == 2)
		{
			WidgetHotbar.HotBarMainMenu = this;
		}
		if (this.idHotbar == 3)
		{
			WidgetHotbar.HotbarBuild = this;
		}
		if (this.idHotbar == 5)
		{
			WidgetHotbar.HotbarExtra = this;
		}
		if (this.extra.rows == 0)
		{
			this.extra.rows = this.hotbar.itemsPerPage;
		}
		this.hotbar.actor = this;
		this.Rebuild();
	}

	public override void OnChangeActionMode()
	{
		base.OnChangeActionMode();
		if (this.hotbar.dirty)
		{
			this.hotbar.dirty = false;
			this.Rebuild();
		}
		this.RefreshHighlight();
	}

	public void Rebuild()
	{
		this.buttons.Clear();
		this.layout.cellSize = EMono.setting.ui.iconSizes[this.extra.iconSize];
		this.layout.constraintCount = this.extra.width;
		this.layout.constraint = (this.extra.vertical ? GridLayoutGroup.Constraint.FixedColumnCount : GridLayoutGroup.Constraint.FixedRowCount);
		this.layout.startCorner = (this.extra.reverse ? GridLayoutGroup.Corner.LowerRight : GridLayoutGroup.Corner.UpperLeft);
		int num = this.extra.rows;
		if (this.extra.autoSize)
		{
			num = 0;
			int num2 = 0;
			while (num2 < this.hotbar.CurrentPage.items.Count && this.hotbar.CurrentPage.items[num2] != null)
			{
				num++;
				num2++;
			}
		}
		int num3 = (num - 1) / this.extra.width + 1;
		int num4 = this.extra.vertical ? this.extra.width : num3;
		int num5 = this.extra.vertical ? num3 : this.extra.width;
		this.imageGrid.uvRect = new Rect(1f, 1f, (float)num4, (float)num5);
		this.layout.DestroyChildren(false, true);
		for (int i = 0; i < num; i++)
		{
			ButtonHotItem buttonHotItem = Util.Instantiate<ButtonHotItem>(this.mold, this.layout);
			buttonHotItem.index = i;
			buttonHotItem.mainText.text = ((i > 10) ? "" : ((i + 1).ToString() ?? ""));
			buttonHotItem.widget = this;
			this.buttons.Add(buttonHotItem);
			if (this.useMask)
			{
				buttonHotItem.gameObject.AddComponent<RectMask2D>();
			}
		}
		this.layout.RebuildLayout(false);
		this.RebuildLayout(false);
		this.RebuildPage(-1);
	}

	public void RebuildPage(int page = -1)
	{
		this.hotbar.SetPage((page == -1) ? this.hotbar.currentPage : page);
		foreach (ButtonHotItem buttonHotItem in this.buttons)
		{
			HotItem item = this.hotbar.GetItem(buttonHotItem.index, -1);
			buttonHotItem.SetItem(item);
		}
		this.SetVisible();
		this.RefreshHighlight();
	}

	public static void RebuildPages()
	{
		foreach (Widget widget in EMono.ui.widgets.list)
		{
			WidgetHotbar widgetHotbar = widget as WidgetHotbar;
			if (widgetHotbar)
			{
				widgetHotbar.RebuildPage(-1);
			}
		}
	}

	public void SwitchPage()
	{
		SE.ClickGeneral();
		this.RebuildPage((this.hotbar.currentPage == 0) ? 1 : 0);
	}

	public HotItem GetItem(int index)
	{
		return this.hotbar.CurrentPage.items.TryGet(index, true);
	}

	public void TryUse(int index)
	{
		HotItem item = this.GetItem(index);
		if (item == null)
		{
			SE.BeepSmall();
			return;
		}
		item.OnClick(item.button, this.hotbar);
	}

	public void OnClickEmptyItem(ButtonHotItem b)
	{
		UIContextMenu uicontextMenu = EMono.ui.CreateContextMenu("ContextMenu");
		this.SetShortcutMenu(b, uicontextMenu);
		uicontextMenu.Show();
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		ButtonHotItem b = InputModuleEX.GetComponentOf<ButtonHotItem>();
		if (this.showThisWidget || !b || b.item == null || base.IsSealed || b.widget.hotbar.IsLocked)
		{
			this.showThisWidget = false;
			SkinSet skin = base.config.skin.Skin;
			UIContextMenu uicontextMenu = m.AddChild("setting");
			UIContextMenu uicontextMenu2 = m.AddChild("style");
			if (!base.IsSealed)
			{
				uicontextMenu.AddSlider("numSlot", (float n) => n.ToString() ?? "", (float)this.extra.rows, delegate(float a)
				{
					this.extra.rows = (int)a;
					this.hotbar.SetSlotNum((int)a);
					this.Rebuild();
					this.ClampToScreen();
				}, 1f, 20f, true, false, false);
			}
			uicontextMenu2.AddSlider("iconSize", (float n) => n.ToString() ?? "", (float)this.extra.iconSize, delegate(float a)
			{
				this.extra.iconSize = (int)a;
				this.Rebuild();
				this.ClampToScreen();
			}, 0f, (float)(EMono.setting.ui.iconSizes.Count - 1), true, true, false);
			base.SetGridContextMenu(uicontextMenu2);
			uicontextMenu.AddToggle("vertical", this.extra.vertical, delegate(bool a)
			{
				this.extra.vertical = a;
				this.Rebuild();
				this.ClampToScreen();
			});
			uicontextMenu.AddToggle("doubleBar", this.extra.width == 2, delegate(bool a)
			{
				this.extra.width = (a ? 2 : 1);
				this.Rebuild();
				this.ClampToScreen();
			});
			uicontextMenu.AddToggle("reverseOrder", this.extra.reverse, delegate(bool a)
			{
				this.extra.reverse = a;
				this.Rebuild();
				this.ClampToScreen();
			});
			if (!base.IsSealed)
			{
				uicontextMenu.AddToggle("alwaysShow2", this.extra.alwaysShow, delegate(bool a)
				{
					this.extra.alwaysShow = a;
				});
			}
			base.SetBaseContextMenu(m);
			if (!base.IsSealed)
			{
				Action <>9__12;
				m.AddButton("resetHotbar", delegate()
				{
					string langDetail = "dialogResetHotbar";
					Action actionYes;
					if ((actionYes = <>9__12) == null)
					{
						actionYes = (<>9__12 = delegate()
						{
							EMono.player.hotbars.ResetHotbar(this.hotbar.id);
							SE.Trash();
						});
					}
					Dialog.YesNo(langDetail, actionYes, null, "yes", "no");
				}, true);
				return;
			}
		}
		else
		{
			m.AddButton("removeHotItem", delegate()
			{
				this.hotbar.SetItem(null, b.index, -1, false);
				this.RebuildPage(-1);
			}, true);
			b.item.OnShowContextMenu(m);
			m.AddToggle("alwaysShow", b.item.always, delegate(bool on)
			{
				b.item.always = on;
			});
			Action <>9__13;
			m.AddButton("thisWidget", delegate()
			{
				this.showThisWidget = true;
				List<Action> actionsNextFrame = EMono.core.actionsNextFrame;
				Action item;
				if ((item = <>9__13) == null)
				{
					item = (<>9__13 = delegate()
					{
						this.ShowContextMenu();
					});
				}
				actionsNextFrame.Add(item);
			}, true);
		}
	}

	public void SetShortcutMenu(ButtonHotItem b, UIContextMenu m)
	{
		Action<UIContextMenu, HotItem> action = delegate(UIContextMenu _m, HotItem i)
		{
			_m.AddButton(i.Name, delegate()
			{
				this.SetItem(b, i);
			}, true);
		};
		UIContextMenu arg = m.AddChild("layerShortcuts");
		action(arg, new HotItemLayer
		{
			id = "LayerHelp"
		});
		action(arg, new HotItemLayer
		{
			id = "stash"
		});
		action(arg, new HotItemLayer
		{
			id = "LayerAbility"
		});
		action(arg, new HotItemLayer
		{
			id = "LayerChara"
		});
		action(arg, new HotItemLayer
		{
			id = "LayerJournal"
		});
		action(arg, new HotItemWidget
		{
			id = "Roster"
		});
		action(arg, new HotItemWidget
		{
			id = "Codex"
		});
		action(arg, new HotItemWidget
		{
			id = "Search"
		});
		action(arg, new HotItemWidget
		{
			id = "Tracker"
		});
		action(arg, new HotItemWidget
		{
			id = "Memo"
		});
		action(arg, new HotItemWidget
		{
			id = "QuestTracker"
		});
		arg = m.AddChild("utilShortcuts");
		action(arg, new HotItemToggle
		{
			type = HotItemToggle.Type.ToggleNoRoof
		});
		action(arg, new HotItemToggle
		{
			type = HotItemToggle.Type.muteBGM
		});
		action(arg, new HotItemToggle
		{
			type = HotItemToggle.Type.showBalloon
		});
		action(arg, new HotItemActionAudoDump());
		if (EMono.core.config.test.unsealWidgets)
		{
			arg = m.AddChild("menuShortcuts");
			action(arg, new HotItemContext
			{
				id = "system"
			});
			if (this == this.IsHotbarSpeed)
			{
				arg = m.AddChild("uniqueShortcuts");
				action(arg, new HotItemSpeed
				{
					id = 0
				});
				action(arg, new HotItemSpeed
				{
					id = 1
				});
				action(arg, new HotItemSpeed
				{
					id = 2
				});
				action(arg, new HotItemSpeed
				{
					id = 3
				});
			}
		}
		arg = m.AddChild("specialShortcuts");
		action(arg, new HotItemTogglePage());
		action(arg, new HotItemToggleVisible());
		m.AddButton("registerPos".lang(), delegate()
		{
			this.SetItem(b, new HotItemFocusPos
			{
				zone = EMono.game.activeZone,
				x = EMono.pc.pos.x,
				y = EMono.pc.pos.z
			});
		}, true);
		m.AddButton("hotActionEQSet".lang(), delegate()
		{
			this.SetItem(b, new HotItemEQSet().Register());
		}, true);
		m.AddButton("hotActionSleep".lang(), delegate()
		{
			this.SetItem(b, new HotItemActionSleep());
		}, true);
	}

	public void SetItem(ButtonHotItem b, HotItem item)
	{
		item = this.hotbar.SetItem(item, b.index, -1, false);
		b.SetItem(item);
		this.RefreshHighlight();
		this.SetVisible();
	}

	public static void RefreshHighlights()
	{
		foreach (Widget widget in EMono.ui.widgets.list)
		{
			WidgetHotbar widgetHotbar = widget as WidgetHotbar;
			if (widgetHotbar)
			{
				widgetHotbar.RefreshHighlight();
			}
		}
	}

	public static void RefreshButtons()
	{
		foreach (Widget widget in EMono.ui.widgets.list)
		{
			WidgetHotbar widgetHotbar = widget as WidgetHotbar;
			if (widgetHotbar)
			{
				foreach (ButtonHotItem buttonHotItem in widgetHotbar.buttons)
				{
					buttonHotItem.RefreshItem();
				}
			}
		}
	}

	public bool RefreshHighlight()
	{
		bool result = false;
		foreach (ButtonHotItem buttonHotItem in this.buttons)
		{
			if (buttonHotItem.item != null)
			{
				if (buttonHotItem.item.UseIconForHighlight)
				{
					if (buttonHotItem.item.ShouldHighlight())
					{
						result = true;
						buttonHotItem.icon.sprite = buttonHotItem.item.GetSprite(true);
					}
					else
					{
						buttonHotItem.icon.sprite = buttonHotItem.item.GetSprite();
					}
				}
				else if (buttonHotItem.item.ShouldHighlight())
				{
					result = true;
					buttonHotItem.image.sprite = buttonHotItem.item.SpriteHighlight;
					if (!this.Visible && buttonHotItem.item.KeepVisibleWhenHighlighted)
					{
						this.ToggleVisible();
					}
				}
				else
				{
					buttonHotItem.image.sprite = EMono.core.refs.spritesHighlight[0];
				}
			}
		}
		return result;
	}

	public void _OnDirtyInventory()
	{
		foreach (ButtonHotItem buttonHotItem in this.buttons)
		{
			if (buttonHotItem.item != null)
			{
				buttonHotItem.Refresh();
			}
		}
	}

	public void SetVisible()
	{
		UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
		int num = 0;
		bool flag = false;
		foreach (ButtonHotItem buttonHotItem in this.buttons)
		{
			bool flag2 = buttonHotItem.item is HotItemToggleVisible;
			bool flag3 = this.Visible || flag2 || (buttonHotItem.item != null && buttonHotItem.item.always);
			buttonHotItem.image.enabled = flag3;
			buttonHotItem.mainText.enabled = flag3;
			buttonHotItem.icon.SetActive(flag3);
			if (!flag2 && buttonHotItem != componentOf)
			{
				buttonHotItem.DoNormalTransition(true);
			}
			if (flag3)
			{
				num++;
			}
			if (flag2)
			{
				flag = true;
			}
		}
		if (num == 0 && !this.Visible)
		{
			this.Visible = true;
			this.SetVisible();
		}
		base.CancelInvoke("CheckAutoHide");
		if (flag)
		{
			this.timeSinceBecomeVisible = 0f;
			base.InvokeRepeating("CheckAutoHide", 0.1f, 0.2f);
		}
		this.imageBG.enabled = (this.imageGrid.enabled = this.extra.visible);
		this.dragPanel.SetActive(this.extra.visible);
	}

	public void ToggleVisible()
	{
		this.extra.visible = !this.extra.visible;
		this.SetVisible();
		if (this.Visible)
		{
			this.imageBG.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
			this.imageBG.transform.DOScale(1f, 0.12f).SetEase(Ease.Linear);
			this.imageGrid.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
			this.imageGrid.transform.DOScale(1f, 0.12f).SetEase(Ease.Linear);
			foreach (ButtonHotItem buttonHotItem in this.buttons)
			{
				if (!(buttonHotItem.item is HotItemToggleVisible) && (buttonHotItem.item == null || !buttonHotItem.item.always))
				{
					buttonHotItem.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
					buttonHotItem.transform.DOScale(1f, 0.12f).SetEase(Ease.Linear);
				}
			}
			this.imageBG.SetAlpha(0f);
			this.imageGrid.SetAlpha(0f);
			SkinConfig skin = base.config.skin;
			this.imageBG.DOFade(skin.bgColor.a, 0.4f).SetEase(Ease.OutQuint);
			this.imageGrid.DOFade(skin.gridColor.a, 0.4f).SetEase(Ease.OutQuint);
		}
	}

	public void CheckAutoHide()
	{
		if (!this.Visible)
		{
			return;
		}
		this.timeSinceBecomeVisible += 0.2f;
		if (this.timeSinceBecomeVisible < 1f)
		{
			return;
		}
		if (Input.GetMouseButton(0) || EMono.ui.contextMenu.currentMenu != null || WidgetHotbar.registering)
		{
			this.timeSinceBecomeVisible = 0f;
			return;
		}
		if (InputModuleEX.IsPointerOver(base.transform))
		{
			this.timeSinceBecomeVisible = 0f;
			return;
		}
		foreach (ButtonHotItem buttonHotItem in this.buttons)
		{
			if (buttonHotItem.item != null && buttonHotItem.item.KeepVisibleWhenHighlighted && buttonHotItem.item.ShouldHighlight())
			{
				return;
			}
		}
		this.ToggleVisible();
	}

	public void OnStartDrag(UIButton b)
	{
		EMono.ui.hud.SetDragImage(b.icon, null, null);
	}

	public void OnDrag(UIButton b)
	{
		string text = "";
		if (this.GetSwapButton(b))
		{
			text = "hotitemSwap";
		}
		else if (!EMono.ui.isPointerOverUI)
		{
			text = "hotitemTrash";
		}
		EMono.ui.hud.SetDragText(text, null);
	}

	public ButtonHotItem GetSwapButton(UIButton b)
	{
		foreach (Widget widget in EMono.ui.widgets.list)
		{
			WidgetHotbar widgetHotbar = widget as WidgetHotbar;
			if (!(widgetHotbar == null))
			{
				foreach (ButtonHotItem buttonHotItem in widgetHotbar.buttons)
				{
					if (InputModuleEX.IsPointerOver(buttonHotItem) && b != buttonHotItem)
					{
						return buttonHotItem;
					}
				}
			}
		}
		return null;
	}

	public void OnEndDrag(UIButton b, bool cancel = false)
	{
		EMono.ui.hud.imageDrag.SetActive(false);
		ButtonHotItem swapButton = this.GetSwapButton(b);
		if (swapButton)
		{
			SE.SelectHotitem();
			ButtonHotItem buttonHotItem = b as ButtonHotItem;
			HotItem hotItem = (HotItem)swapButton.item;
			HotItem hotItem2 = (HotItem)buttonHotItem.item;
			Hotbar hotbar = swapButton.widget.hotbar;
			Hotbar hotbar2 = buttonHotItem.widget.hotbar;
			hotbar.actor.SetItem(swapButton, hotItem2);
			hotbar2.actor.SetItem(buttonHotItem, hotItem);
			if (hotItem != null)
			{
				hotItem.button = buttonHotItem;
				hotItem.hotbar = hotbar2;
			}
			if (hotItem2 != null)
			{
				hotItem2.button = swapButton;
				hotItem2.hotbar = hotbar;
			}
			UIInventory.RefreshAllList();
			return;
		}
		if (EMono.ui.isPointerOverUI)
		{
			SE.Beep();
			return;
		}
		SE.Trash();
		this.SetItem(b as ButtonHotItem, null);
		UIInventory.RefreshAllList();
	}

	public static bool registering;

	public static bool dirtyCurrentItem;

	public static HotItem registeringItem;

	public static WidgetHotbar HotbarBuild;

	public static WidgetHotbar HotBarMainMenu;

	public static WidgetHotbar HotbarExtra;

	public int idHotbar;

	public GridLayoutGroup layout;

	public ButtonHotItem mold;

	public RawImage imageGrid;

	public Image imageSelect;

	public Hotbar.Type type;

	public bool useMask;

	[NonSerialized]
	public List<ButtonHotItem> buttons = new List<ButtonHotItem>();

	private bool showThisWidget;

	private float timeSinceBecomeVisible;

	public class Extra
	{
		public int iconSize = 2;

		public int width = 1;

		public int rows;

		public bool showShortcut;

		public bool vertical;

		public bool visible;

		public bool alwaysShow;

		public bool reverse;

		public bool autoSize;
	}
}
