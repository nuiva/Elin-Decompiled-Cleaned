using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInventory : EMono
{
	public InvOwner owner
	{
		get
		{
			return (this.currentTab ?? this.tabs[0]).owner;
		}
	}

	public Card dest
	{
		get
		{
			return this.currentTab.dest;
		}
	}

	public bool UseBG
	{
		get
		{
			return this.window.saveData.useBG || !this.UseGrid;
		}
	}

	public bool UseGrid
	{
		get
		{
			return EMono.core.config.game.useGrid;
		}
	}

	public bool IsMagicChest
	{
		get
		{
			return this.owner.Container.trait is TraitMagicChest;
		}
	}

	public bool IsToolbelt
	{
		get
		{
			return this.owner.Container.trait is TraitToolBelt;
		}
	}

	public bool IsShop
	{
		get
		{
			return this.owner.Container.trait is TraitChestMerchant;
		}
	}

	public CoreRef.InventoryStyle InvStyle
	{
		get
		{
			return EMono.core.refs.invStyle[(this.UseBG || this.IsToolbelt) ? this.owner.Container.trait.IDInvStyle : "transparent"];
		}
	}

	public bool IsMainMode
	{
		get
		{
			switch (this.currentTab.mode)
			{
			case UIInventory.Mode.Gear:
			case UIInventory.Mode.Food:
			case UIInventory.Mode.Item:
			case UIInventory.Mode.Resource:
			case UIInventory.Mode.Read:
			case UIInventory.Mode.Hold:
			case UIInventory.Mode.Tool:
			case UIInventory.Mode.All:
			case UIInventory.Mode.Drink:
			case UIInventory.Mode.HoldFurniture:
			case UIInventory.Mode.HoldBlock:
				return true;
			}
			return false;
		}
	}

	public CurrencyType CurrencyType
	{
		get
		{
			return this.currentTab.owner.currency;
		}
	}

	public string IDCurrency
	{
		get
		{
			CurrencyType currencyType = this.CurrencyType;
			if (currencyType == CurrencyType.Medal)
			{
				return "medal";
			}
			if (currencyType != CurrencyType.Ecopo)
			{
				return "money";
			}
			return "ecopo";
		}
	}

	public static void RefreshAllList()
	{
		UIInventory[] componentsInChildren = EMono.ui.rectLayers.GetComponentsInChildren<UIInventory>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].list.Redraw();
		}
	}

	public void SetHeader(string s)
	{
		this.window.SetCaption(s.lang());
	}

	public UIInventory.Tab AddTab(Card c, UIInventory.Mode mode, Thing container = null)
	{
		return this.AddTab(new InvOwner(c, container, CurrencyType.None, PriceType.Default), mode);
	}

	public UIInventory.Tab AddTab(InvOwner owner, UIInventory.Mode mode = UIInventory.Mode.All)
	{
		owner.Init();
		UIInventory.Tab tab = new UIInventory.Tab
		{
			mode = mode,
			owner = owner
		};
		this.tabs.Add(tab);
		return tab;
	}

	public void OnInit()
	{
		this.interactMode = EMono.player.pref.interactMode;
		using (List<UIInventory.Tab>.Enumerator enumerator = this.tabs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				UIInventory.Tab t = enumerator.Current;
				string text = t.textTab.IsEmpty("inv" + t.mode.ToString());
				if (!this.floatMode)
				{
					string str = text.lang();
					string str2 = " - ";
					string str3;
					if (!t.owner.owner.IsPC)
					{
						Chara chara = t.owner.Chara;
						str3 = (((chara != null) ? chara.Name : null) ?? t.owner.Container.Name);
					}
					else
					{
						str3 = "you".lang().ToTitleCase(false);
					}
					text = str + str2 + str3;
				}
				Window.Setting.Tab tab = this.window.AddTab(text, this.content, delegate
				{
					this.SwitchTab(t);
				}, EMono.core.refs.icons.invTab.TryGetValue(t.mode, null), text);
				if (t.smallTab)
				{
					tab.customTab = this.moldSmallTab;
				}
			}
		}
		if (this.layer.mini)
		{
			this.layer.mini.SetActive(this.owner.Container.isChara);
			if (this.owner.Container.isChara)
			{
				this.layer.mini.SetChara(this.owner.Container.Chara);
			}
		}
		if (this.uiMagic)
		{
			this.uiMagic.SetActive(this.IsMagicChest);
			if (this.IsMagicChest)
			{
				this.uiMagic.Init();
			}
		}
		if (this.IsMagicChest)
		{
			TooltipManager.Instance.disableTimer = 0.1f;
		}
	}

	public void RefreshWindow()
	{
		if (this.window.saveData == null)
		{
			Window window = this.window;
			Window.SaveData saveData;
			if ((saveData = Window.dictData.TryGetValue(this.window.idWindow, null)) == null)
			{
				(saveData = new Window.SaveData()).useBG = EMono.core.config.game.showInvBG;
			}
			window.saveData = saveData;
		}
		this.window.setting.allowMove = !this.window.saveData.fixedPos;
		this.window.bgCollider.raycastTarget = (!this.window.saveData.fixedPos || this.window.saveData.useBG);
		this.window.imageBG.SetActive(this.UseBG);
		CoreRef.InventoryStyle inventoryStyle = EMono.core.refs.invStyle[this.owner.Container.trait.IDInvStyle];
		if (this.UseBG)
		{
			this.window.imageBG.sprite = inventoryStyle.bg;
			this.window.imageBG.rectTransform.anchoredPosition = inventoryStyle.posFix;
			this.window.imageBG.rectTransform.sizeDelta = inventoryStyle.sizeDelta;
			this.window.imageBG.alphaHitTestMinimumThreshold = 1f;
		}
		if (this.IsToolbelt)
		{
			this.window.cgFloatMenu.SetActive(false);
			this.window.cgFloatMenu = null;
			this.window.bgCollider.rectTransform.anchoredPosition = new Vector2(-2.5f, 0f);
			this.window.bgCollider.rectTransform.sizeDelta = new Vector2(-20f, -20f);
		}
		if (EMono.core.config.ui.showFloatButtons && this.window.cgFloatMenu)
		{
			this.window.cgFloatMenu.alpha = 1f;
			this.window.cgFloatMenu.enabled = false;
			this.window.cgFloatMenu.SetActive(true);
		}
	}

	public void Close()
	{
		if (!this.floatMode)
		{
			this.layer.Close();
		}
	}

	public void SwitchTab(UIInventory.Tab tab)
	{
		if (this.currentTab == tab)
		{
			return;
		}
		this.currentTab = tab;
		this.RefreshWindow();
		this.destNum = (this.lastNum = (this.DestNumIsMax() ? -1 : 0));
		if (this.isList)
		{
			this.RefreshList();
		}
		else
		{
			this.RefreshGrid();
		}
		if (this.IsShop || this.window.saveData.alwaysSort)
		{
			this.Sort(true);
		}
		if (this.layer.uiCurrency)
		{
			tab.owner.BuildUICurrency(this.layer.uiCurrency, tab.owner.owner.trait.CostRerollShop != 0);
		}
		this.layer.TryShowHint("h_inv" + tab.mode.ToString());
		if (this.headerRow)
		{
			UIHeader[] componentsInChildren = this.headerRow.GetComponentsInChildren<UIHeader>(true);
			componentsInChildren[0].SetText("headerItem");
			UIInventory.Mode mode = tab.mode;
			if (mode - UIInventory.Mode.Buy <= 1 || mode == UIInventory.Mode.Identify)
			{
				componentsInChildren[1].SetText("headerPrice".lang(this.IDCurrency.lang(), null, null, null, null));
				return;
			}
			if (mode != UIInventory.Mode.Recycle)
			{
				componentsInChildren[1].SetText("headerWeight");
				return;
			}
			componentsInChildren[1].SetText("headerRecycle");
		}
	}

	public void DoAct(Act act)
	{
		if (!EMono.pc.HasNoGoal || (act.LocalAct && EMono._zone.IsRegion))
		{
			SE.Beep();
			return;
		}
		EMono.player.hotItemToRestore = EMono.player.currentHotItem;
		if (act.IsAct)
		{
			act.Perform(EMono.pc, null, null);
			return;
		}
		this.currentAct = (act as AIAct);
		EMono.pc.SetAI(this.currentAct);
		ActionMode.Adv.SetTurbo(-1);
		if (!this.floatMode)
		{
			this.Close();
			return;
		}
	}

	private void Update()
	{
		if (this.owner.Container.isDestroyed)
		{
			this.Close();
			return;
		}
		this.imageHighlightGrid.SetActive(LayerInventory.highlightInv == this.owner);
		this.CheckDirty();
		if (EInput.action == EAction.GetAll && this.buttonTakeAll && !this.IsMagicChest)
		{
			this.buttonTakeAll.onClick.Invoke();
		}
		if ((this.IsMagicChest || this.window.saveData.alwaysSort) && this.wasDirty && (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)))
		{
			this.list.Redraw();
			if (!Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
			{
				this.wasDirty = false;
				UIButton.TryShowTip(null, true, true);
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			this.firstMouseRightDown = true;
		}
	}

	public void CheckDirty()
	{
		if (this.dirty)
		{
			this.wasDirty = true;
			if (this.UseGrid)
			{
				this.list.Redraw();
			}
			else
			{
				this.list.List(false);
			}
			UIButton.TryShowTip(null, true, true);
		}
	}

	public void RefreshDestNum()
	{
		if (this.destNum > 0)
		{
			return;
		}
		this.destNum = (this.DestNumIsMax() ? (EInput.isShiftDown ? 0 : -1) : (EInput.isShiftDown ? -1 : 0));
		if (this.destNum != this.lastNum)
		{
			this.lastNum = this.destNum;
			this.list.Redraw();
		}
	}

	public bool DestNumIsMax()
	{
		UIInventory.Mode mode = this.currentTab.mode;
		return mode - UIInventory.Mode.Buy > 1;
	}

	public void RefreshMenu()
	{
		UIInventory.<>c__DisplayClass69_0 CS$<>8__locals1 = new UIInventory.<>c__DisplayClass69_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.b = this.window.buttonSort;
		CS$<>8__locals1.data = this.window.saveData;
		if (CS$<>8__locals1.b)
		{
			CS$<>8__locals1.b.onClick.RemoveAllListeners();
			CS$<>8__locals1.b.onClick.AddListener(delegate()
			{
				UIInventory.<>c__DisplayClass69_1 CS$<>8__locals4 = new UIInventory.<>c__DisplayClass69_1();
				CS$<>8__locals4.CS$<>8__locals1 = CS$<>8__locals1;
				UIContextMenu uicontextMenu = EMono.ui.CreateContextMenuInteraction();
				uicontextMenu.layoutGroup.childAlignment = TextAnchor.UpperLeft;
				uicontextMenu.alwaysPopLeft = true;
				UIContextMenu uicontextMenu2 = uicontextMenu.AddChild("sort", TextAnchor.UpperRight);
				UIList.SortMode[] sorts = CS$<>8__locals1.<>4__this.list.sorts;
				for (int i = 0; i < sorts.Length; i++)
				{
					UIList.SortMode sort = sorts[i];
					UIList.SortMode _sort = sort;
					uicontextMenu2.AddButton((((CS$<>8__locals1.<>4__this.IsShop ? EMono.player.pref.sortInvShop : EMono.player.pref.sortInv) == _sort) ? "context_checker".lang() : "") + _sort.ToString().lang(), delegate()
					{
						if (CS$<>8__locals4.CS$<>8__locals1.<>4__this.IsShop)
						{
							EMono.player.pref.sortInvShop = _sort;
						}
						else
						{
							EMono.player.pref.sortInv = _sort;
						}
						CS$<>8__locals4.CS$<>8__locals1.<>4__this.Sort(true);
						SE.Click();
					}, true);
				}
				UIContextMenu uicontextMenu3 = uicontextMenu2;
				string idLang = "sort_ascending";
				bool isOn = CS$<>8__locals1.<>4__this.IsShop ? EMono.player.pref.sort_ascending_shop : EMono.player.pref.sort_ascending;
				UnityAction<bool> action;
				if ((action = CS$<>8__locals1.<>9__3) == null)
				{
					action = (CS$<>8__locals1.<>9__3 = delegate(bool a)
					{
						if (CS$<>8__locals1.<>4__this.IsShop)
						{
							EMono.player.pref.sort_ascending_shop = a;
						}
						else
						{
							EMono.player.pref.sort_ascending = a;
						}
						CS$<>8__locals1.<>4__this.Sort(true);
						SE.Click();
					});
				}
				uicontextMenu3.AddToggle(idLang, isOn, action);
				if (!CS$<>8__locals1.<>4__this.IsMagicChest)
				{
					UIContextMenu uicontextMenu4 = uicontextMenu2;
					string idLang2 = "sort_always";
					bool alwaysSort = CS$<>8__locals1.data.alwaysSort;
					UnityAction<bool> action2;
					if ((action2 = CS$<>8__locals1.<>9__4) == null)
					{
						action2 = (CS$<>8__locals1.<>9__4 = delegate(bool a)
						{
							CS$<>8__locals1.data.alwaysSort = a;
							if (CS$<>8__locals1.data.alwaysSort)
							{
								CS$<>8__locals1.<>4__this.Sort(true);
							}
							SE.Click();
						});
					}
					uicontextMenu4.AddToggle(idLang2, alwaysSort, action2);
				}
				if (CS$<>8__locals1.<>4__this.IsMagicChest)
				{
					UIContextMenu uicontextMenu5 = uicontextMenu.AddChild("catFilterType", TextAnchor.UpperRight);
					foreach (Window.SaveData.CategoryType categoryType in Util.EnumToList<Window.SaveData.CategoryType>())
					{
						Window.SaveData.CategoryType _c = categoryType;
						uicontextMenu5.AddButton(((CS$<>8__locals1.data.category == categoryType) ? "context_checker".lang() : "") + ("catFilterType_" + _c.ToString()).lang(), delegate()
						{
							CS$<>8__locals4.CS$<>8__locals1.data.category = _c;
							CS$<>8__locals4.CS$<>8__locals1.<>4__this.uiMagic.idCat = "";
							CS$<>8__locals4.CS$<>8__locals1.<>4__this.list.Redraw();
							SE.Click();
						}, true);
					}
				}
				CS$<>8__locals4.con = CS$<>8__locals1.<>4__this.owner.Container;
				bool flag2 = (!CS$<>8__locals4.con.isNPCProperty && !CS$<>8__locals4.con.isChara && (CS$<>8__locals4.con.trait is TraitShippingChest || (CS$<>8__locals4.con.GetRoot() is Zone && EMono._zone.IsPCFaction) || CS$<>8__locals4.con.GetRootCard() == EMono.pc)) || EMono._zone is Zone_Tent;
				if (CS$<>8__locals4.con.IsPC)
				{
					flag2 = true;
				}
				if (CS$<>8__locals4.con.trait is TraitChestMerchant)
				{
					flag2 = false;
				}
				string text;
				if (flag2)
				{
					UIInventory.<>c__DisplayClass69_4 CS$<>8__locals7 = new UIInventory.<>c__DisplayClass69_4();
					CS$<>8__locals7.CS$<>8__locals4 = CS$<>8__locals4;
					CS$<>8__locals7.dis = uicontextMenu.AddChild("distribution", TextAnchor.UpperRight);
					UIContextMenu dis = CS$<>8__locals7.dis;
					text = "priority_hint";
					Func<float, string> textFunc = (float a) => a.ToString() ?? "";
					float value = (float)CS$<>8__locals1.data.priority;
					Action<float> action3;
					if ((action3 = CS$<>8__locals1.<>9__15) == null)
					{
						action3 = (CS$<>8__locals1.<>9__15 = delegate(float a)
						{
							CS$<>8__locals1.data.priority = (int)a;
						});
					}
					dis.AddSlider(text, textFunc, value, action3, -5f, 20f, true, false, false);
					CS$<>8__locals7.dist = CS$<>8__locals1.<>4__this.ShowDistribution(CS$<>8__locals7.dis, CS$<>8__locals1.data);
					CS$<>8__locals7.distAdv = CS$<>8__locals1.<>4__this.ShowAdvDistribution(CS$<>8__locals7.dis, CS$<>8__locals1.data);
					CS$<>8__locals7.<RefreshMenu>g__RefreshDist|16();
					if (CS$<>8__locals7.CS$<>8__locals4.con.trait.IsFridge || EMono.core.config.game.advancedMenu)
					{
						UIContextMenu dis2 = CS$<>8__locals7.dis;
						string idLang3 = "onlyRottable";
						bool onlyRottable = CS$<>8__locals1.data.onlyRottable;
						UnityAction<bool> action4;
						if ((action4 = CS$<>8__locals1.<>9__17) == null)
						{
							action4 = (CS$<>8__locals1.<>9__17 = delegate(bool a)
							{
								CS$<>8__locals1.data.onlyRottable = a;
								SE.ClickOk();
							});
						}
						dis2.AddToggle(idLang3, onlyRottable, action4);
					}
					UIContextMenu dis3 = CS$<>8__locals7.dis;
					string idLang4 = "noRotten";
					bool noRotten = CS$<>8__locals1.data.noRotten;
					UnityAction<bool> action5;
					if ((action5 = CS$<>8__locals1.<>9__18) == null)
					{
						action5 = (CS$<>8__locals1.<>9__18 = delegate(bool a)
						{
							CS$<>8__locals1.data.noRotten = a;
							SE.ClickOk();
						});
					}
					dis3.AddToggle(idLang4, noRotten, action5);
					CS$<>8__locals7.dis.AddToggle("advDistribution", CS$<>8__locals1.data.advDistribution, delegate(bool a)
					{
						CS$<>8__locals7.CS$<>8__locals4.CS$<>8__locals1.data.advDistribution = a;
						base.<RefreshMenu>g__RefreshDist|16();
						SE.ClickOk();
					});
					if (EMono.core.config.game.advancedMenu)
					{
						UIContextMenu dis4 = CS$<>8__locals7.dis;
						string idLang5 = CS$<>8__locals1.data.filter.IsEmpty() ? "distFilter" : "distFilter2".lang(CS$<>8__locals1.data.filter, null, null, null, null);
						Action action6;
						if ((action6 = CS$<>8__locals1.<>9__20) == null)
						{
							action6 = (CS$<>8__locals1.<>9__20 = delegate()
							{
								string langDetail = "distFilter3";
								string text5 = CS$<>8__locals1.data.filter.IsEmpty("");
								Action<bool, string> onClose;
								if ((onClose = CS$<>8__locals1.<>9__21) == null)
								{
									onClose = (CS$<>8__locals1.<>9__21 = delegate(bool cancel, string s)
									{
										if (!cancel)
										{
											CS$<>8__locals1.data._filterStrs = null;
											CS$<>8__locals1.data.filter = s;
										}
									});
								}
								Dialog.InputName(langDetail, text5, onClose, Dialog.InputType.DistributionFilter);
							});
						}
						dis4.AddButton(idLang5, action6, true);
					}
				}
				if (CS$<>8__locals4.con.IsPC && EMono.core.config.game.advancedMenu)
				{
					UIInventory.<>c__DisplayClass69_5 CS$<>8__locals8 = new UIInventory.<>c__DisplayClass69_5();
					CS$<>8__locals8.data = EMono.player.dataPick;
					CS$<>8__locals8.dis = uicontextMenu.AddChild("autopick", TextAnchor.UpperRight);
					CS$<>8__locals8.dist = CS$<>8__locals1.<>4__this.ShowDistribution(CS$<>8__locals8.dis, CS$<>8__locals8.data);
					CS$<>8__locals8.distAdv = CS$<>8__locals1.<>4__this.ShowAdvDistribution(CS$<>8__locals8.dis, CS$<>8__locals8.data);
					CS$<>8__locals8.<RefreshMenu>g__RefreshDist|22();
					CS$<>8__locals8.dis.AddToggle("noRotten", CS$<>8__locals8.data.noRotten, delegate(bool a)
					{
						CS$<>8__locals8.data.noRotten = a;
						SE.ClickOk();
					});
					CS$<>8__locals8.dis.AddToggle("advDistribution", CS$<>8__locals8.data.advDistribution, delegate(bool a)
					{
						CS$<>8__locals8.data.advDistribution = a;
						base.<RefreshMenu>g__RefreshDist|22();
						SE.ClickOk();
					});
					if (EMono.core.config.game.advancedMenu)
					{
						CS$<>8__locals8.dis.AddButton(CS$<>8__locals8.data.filter.IsEmpty() ? "distFilter" : "distFilter2".lang(CS$<>8__locals8.data.filter, null, null, null, null), delegate()
						{
							string langDetail = "distFilter3";
							string text5 = CS$<>8__locals8.data.filter.IsEmpty("");
							Action<bool, string> onClose;
							if ((onClose = CS$<>8__locals8.<>9__26) == null)
							{
								onClose = (CS$<>8__locals8.<>9__26 = delegate(bool cancel, string s)
								{
									if (!cancel)
									{
										CS$<>8__locals8.data._filterStrs = null;
										CS$<>8__locals8.data.filter = s;
									}
								});
							}
							Dialog.InputName(langDetail, text5, onClose, Dialog.InputType.DistributionFilter);
						}, true);
					}
				}
				if (CS$<>8__locals4.con.trait is TraitShippingChest || (CS$<>8__locals4.con.IsInstalled && (EMono._zone.IsPCFaction || EMono._zone is Zone_Tent)))
				{
					UIContextMenu uicontextMenu6 = uicontextMenu.AddChild("autodump", TextAnchor.UpperRight);
					foreach (AutodumpFlag autodumpFlag in new List<AutodumpFlag>
					{
						AutodumpFlag.existing,
						AutodumpFlag.sameCategory,
						AutodumpFlag.distribution,
						AutodumpFlag.none
					})
					{
						string str = (CS$<>8__locals1.data.autodump == autodumpFlag) ? "context_checker".lang() : "";
						AutodumpFlag _e = autodumpFlag;
						UIButton uibutton = uicontextMenu6.AddButton(str + ("dump_" + autodumpFlag.ToString()).lang(), delegate()
						{
							SE.Click();
							CS$<>8__locals4.CS$<>8__locals1.data.autodump = _e;
						}, true);
						if (autodumpFlag != AutodumpFlag.none)
						{
							uibutton.SetTooltipLang("dump_" + autodumpFlag.ToString() + "_tip");
						}
					}
				}
				if (CS$<>8__locals4.con.IsPC || (CS$<>8__locals4.con.isThing && !(CS$<>8__locals4.con.trait is TraitChestMerchant) && !CS$<>8__locals4.con.isNPCProperty))
				{
					UIContextMenu uicontextMenu7 = uicontextMenu.AddChild("config", TextAnchor.UpperRight);
					UIContextMenu uicontextMenu8 = uicontextMenu7;
					string idLang6 = "toggleExcludeCraft";
					bool excludeCraft = CS$<>8__locals1.data.excludeCraft;
					UnityAction<bool> action7;
					if ((action7 = CS$<>8__locals1.<>9__28) == null)
					{
						action7 = (CS$<>8__locals1.<>9__28 = delegate(bool a)
						{
							CS$<>8__locals1.data.excludeCraft = a;
							SE.ClickOk();
						});
					}
					uicontextMenu8.AddToggle(idLang6, excludeCraft, action7);
					if (CS$<>8__locals4.con.GetRootCard() == EMono.pc)
					{
						UIContextMenu uicontextMenu9 = uicontextMenu7;
						string idLang7 = "toggleDump";
						bool excludeDump = CS$<>8__locals1.data.excludeDump;
						UnityAction<bool> action8;
						if ((action8 = CS$<>8__locals1.<>9__29) == null)
						{
							action8 = (CS$<>8__locals1.<>9__29 = delegate(bool a)
							{
								CS$<>8__locals1.data.excludeDump = a;
								SE.ClickOk();
							});
						}
						uicontextMenu9.AddToggle(idLang7, excludeDump, action8);
					}
					if (EMono.core.config.game.advancedMenu)
					{
						if (!CS$<>8__locals4.con.IsPC)
						{
							UIContextMenu uicontextMenu10 = uicontextMenu7;
							string idLang8 = "noRightClickClose";
							bool noRightClickClose = CS$<>8__locals1.data.noRightClickClose;
							UnityAction<bool> action9;
							if ((action9 = CS$<>8__locals1.<>9__30) == null)
							{
								action9 = (CS$<>8__locals1.<>9__30 = delegate(bool a)
								{
									CS$<>8__locals1.data.noRightClickClose = a;
									SE.ClickOk();
								});
							}
							uicontextMenu10.AddToggle(idLang8, noRightClickClose, action9);
						}
						UIContextMenu uicontextMenu11 = uicontextMenu7;
						string idLang9 = "fixedPos";
						bool fixedPos = CS$<>8__locals1.data.fixedPos;
						UnityAction<bool> action10;
						if ((action10 = CS$<>8__locals1.<>9__31) == null)
						{
							action10 = (CS$<>8__locals1.<>9__31 = delegate(bool a)
							{
								CS$<>8__locals1.data.fixedPos = a;
								CS$<>8__locals1.<>4__this.RefreshWindow();
								SE.ClickOk();
							});
						}
						uicontextMenu11.AddToggle(idLang9, fixedPos, action10);
						UIContextMenu uicontextMenu12 = uicontextMenu7;
						string idLang10 = "toggleItemCompress";
						bool compress = CS$<>8__locals1.data.compress;
						UnityAction<bool> action11;
						if ((action11 = CS$<>8__locals1.<>9__32) == null)
						{
							action11 = (CS$<>8__locals1.<>9__32 = delegate(bool a)
							{
								CS$<>8__locals1.data.compress = a;
								SE.ClickOk();
							});
						}
						uicontextMenu12.AddToggle(idLang10, compress, action11);
					}
					if (CS$<>8__locals4.con.IsPC)
					{
						uicontextMenu7.AddToggle("placeContainerCenter", EMono.player.openContainerCenter, delegate(bool a)
						{
							EMono.player.openContainerCenter = a;
							SE.ClickOk();
						});
					}
					if (!CS$<>8__locals4.con.isChara && !CS$<>8__locals4.con.trait.IsSpecialContainer)
					{
						uicontextMenu7.AddButton("changeName", delegate()
						{
							string langDetail = "dialogChangeName";
							string text5 = CS$<>8__locals4.con.c_altName.IsEmpty("");
							Action<bool, string> onClose;
							if ((onClose = CS$<>8__locals4.<>9__37) == null)
							{
								onClose = (CS$<>8__locals4.<>9__37 = delegate(bool cancel, string text)
								{
									if (!cancel)
									{
										CS$<>8__locals4.con.c_altName = text;
									}
								});
							}
							Dialog.InputName(langDetail, text5, onClose, Dialog.InputType.Item);
						}, true);
					}
					if (EMono.core.config.game.advancedMenu)
					{
						uicontextMenu7.AddButton("copyContainer", delegate()
						{
							SE.ClickOk();
							EMono.player.windowDataCopy = IO.DeepCopy<Window.SaveData>(CS$<>8__locals4.CS$<>8__locals1.data);
							EMono.player.windowDataName = CS$<>8__locals4.con.Name;
						}, true);
						if (EMono.player.windowDataCopy != null && !CS$<>8__locals4.con.IsPC)
						{
							UIContextMenu uicontextMenu13 = uicontextMenu7;
							string idLang11 = "pasteContainer".lang(EMono.player.windowDataName, null, null, null, null);
							Action action12;
							if ((action12 = CS$<>8__locals1.<>9__36) == null)
							{
								action12 = (CS$<>8__locals1.<>9__36 = delegate()
								{
									SE.ClickOk();
									CS$<>8__locals1.<>4__this.window.saveData.CopyFrom(EMono.player.windowDataCopy);
									CS$<>8__locals1.<>4__this.RefreshWindow();
									CS$<>8__locals1.<>4__this.RefreshGrid();
								});
							}
							uicontextMenu13.AddButton(idLang11, action12, true);
						}
					}
				}
				UIContextMenu uicontextMenu14 = uicontextMenu.AddChild("appearanceWindow", TextAnchor.UpperRight);
				UIContextMenu uicontextMenu15 = uicontextMenu14;
				string idLang12 = "toggleBG";
				bool useBG = CS$<>8__locals1.data.useBG;
				UnityAction<bool> action13;
				if ((action13 = CS$<>8__locals1.<>9__5) == null)
				{
					action13 = (CS$<>8__locals1.<>9__5 = delegate(bool a)
					{
						CS$<>8__locals1.data.useBG = a;
						CS$<>8__locals1.data.color = CS$<>8__locals1.<>4__this.InvStyle.gridColor;
						CS$<>8__locals1.<>4__this.RefreshWindow();
						CS$<>8__locals1.<>4__this.RefreshGrid();
						SE.ClickOk();
					});
				}
				uicontextMenu15.AddToggle(idLang12, useBG, action13);
				UIContextMenu uicontextMenu16 = uicontextMenu14;
				string text2 = "size";
				Func<float, string> textFunc2 = (float a) => a.ToString() ?? "";
				float value2 = (float)CS$<>8__locals1.data.size;
				Action<float> action14;
				if ((action14 = CS$<>8__locals1.<>9__7) == null)
				{
					action14 = (CS$<>8__locals1.<>9__7 = delegate(float b)
					{
						CS$<>8__locals1.data.size = (int)b;
						CS$<>8__locals1.<>4__this.RefreshGrid();
					});
				}
				uicontextMenu16.AddSlider(text2, textFunc2, value2, action14, -25f, 25f, true, false, false);
				if (EMono.core.config.game.advancedMenu && !CS$<>8__locals1.<>4__this.IsMagicChest)
				{
					UIContextMenu uicontextMenu17 = uicontextMenu14;
					string text3 = "columns";
					Func<float, string> textFunc3 = (float a) => a.ToString() ?? "";
					float value3 = (float)CS$<>8__locals1.data.columns;
					Action<float> action15;
					if ((action15 = CS$<>8__locals1.<>9__9) == null)
					{
						action15 = (CS$<>8__locals1.<>9__9 = delegate(float b)
						{
							CS$<>8__locals1.data.columns = (int)b;
							CS$<>8__locals1.<>4__this.RefreshGrid();
						});
					}
					uicontextMenu17.AddSlider(text3, textFunc3, value3, action15, 0f, 20f, true, false, false);
				}
				UIContextMenu uicontextMenu18 = uicontextMenu14;
				string idLang13 = "colorGrid";
				Action action16;
				if ((action16 = CS$<>8__locals1.<>9__10) == null)
				{
					action16 = (CS$<>8__locals1.<>9__10 = delegate()
					{
						LayerColorPicker layerColorPicker = EMono.ui.AddLayer<LayerColorPicker>();
						Color startColor = CS$<>8__locals1.data.color;
						Color gridColor = CS$<>8__locals1.<>4__this.InvStyle.gridColor;
						Action<PickerState, Color> onChangeColor;
						if ((onChangeColor = CS$<>8__locals1.<>9__38) == null)
						{
							onChangeColor = (CS$<>8__locals1.<>9__38 = delegate(PickerState state, Color _c)
							{
								CS$<>8__locals1.data.color = _c;
								CS$<>8__locals1.<>4__this.list.bgGrid.color = _c;
								if (CS$<>8__locals1.data.color.a == 0)
								{
									CS$<>8__locals1.<>4__this.list.bgGrid.color = CS$<>8__locals1.<>4__this.InvStyle.gridColor;
								}
							});
						}
						layerColorPicker.SetColor(startColor, gridColor, onChangeColor);
					});
				}
				uicontextMenu18.AddButton(idLang13, action16, true);
				if (!CS$<>8__locals4.con.isChara)
				{
					UIContextMenu uicontextMenu19 = uicontextMenu14;
					string idLang14 = "changeIcon";
					Action action17;
					if ((action17 = CS$<>8__locals1.<>9__11) == null)
					{
						action17 = (CS$<>8__locals1.<>9__11 = delegate()
						{
							EMono.ui.contextMenu.currentMenu.Hide();
							UIContextMenu uicontextMenu21 = EMono.ui.CreateContextMenuInteraction();
							GridLayoutGroup parent = uicontextMenu21.AddGridLayout();
							int num = 0;
							foreach (Sprite sprite in EMono.core.refs.spritesContainerIcon)
							{
								UIButton uibutton2 = Util.Instantiate<UIButton>("UI/Element/Button/ButtonContainerIcon", parent);
								int _i = num;
								uibutton2.icon.sprite = EMono.core.refs.spritesContainerIcon[_i];
								uibutton2.SetOnClick(delegate
								{
									SE.Click();
									CS$<>8__locals1.<>4__this.owner.Container.c_indexContainerIcon = _i;
									LayerInventory.SetDirty(CS$<>8__locals1.<>4__this.owner.Container.Thing);
									EMono.ui.contextMenu.currentMenu.Hide();
								});
								num++;
							}
							uicontextMenu21.Show();
						});
					}
					uicontextMenu19.AddButton(idLang14, action17, true);
				}
				if (EMono.debug.enable)
				{
					UIContextMenu uicontextMenu20 = uicontextMenu.AddChild("debug", TextAnchor.UpperRight);
					uicontextMenu20.AddToggle("toggleGrid", EMono.core.config.game.useGrid, delegate(bool a)
					{
						EMono.core.config.game.useGrid = a;
						foreach (LayerInventory layerInventory in LayerInventory.listInv)
						{
							layerInventory.invs[0].RefreshWindow();
							layerInventory.invs[0].RefreshGrid();
						}
					});
					string text4 = "iconSize";
					Func<float, string> textFunc4 = (float a) => a.ToString() ?? "";
					float value4 = (float)EMono.game.config.gridIconSize;
					Action<float> action18;
					if ((action18 = CS$<>8__locals1.<>9__42) == null)
					{
						action18 = (CS$<>8__locals1.<>9__42 = delegate(float b)
						{
							EMono.game.config.gridIconSize = (int)b;
							CS$<>8__locals1.<>4__this.RefreshGrid();
						});
					}
					uicontextMenu20.AddSlider(text4, textFunc4, value4, action18, 100f, 150f, true, false, false);
				}
				uicontextMenu.Show();
				uicontextMenu.hideOnMouseLeave = false;
			});
		}
		CS$<>8__locals1.b = this.window.buttonQuickSort;
		if (CS$<>8__locals1.b)
		{
			CS$<>8__locals1.b.onClick.RemoveAllListeners();
			CS$<>8__locals1.b.onClick.AddListener(delegate()
			{
				CS$<>8__locals1.<>4__this.Sort(true);
				SE.Click();
			});
		}
		CS$<>8__locals1.b = this.window.buttonExtra;
		if (CS$<>8__locals1.b)
		{
			CS$<>8__locals1.b.SetActive(this.owner.Container.IsPC);
			CS$<>8__locals1.b.onClick.RemoveAllListeners();
			CS$<>8__locals1.b.onClick.AddListener(delegate()
			{
				TaskDump.TryPerform();
			});
		}
		CS$<>8__locals1.b = this.window.buttonShared;
		if (CS$<>8__locals1.b)
		{
			Card con = this.owner.Container;
			bool flag = !con.isChara && ((con.IsInstalled && EMono._zone.IsPCFaction) || this.owner.owner.IsPC);
			CS$<>8__locals1.b.SetActive(flag);
			if (flag)
			{
				CS$<>8__locals1.<RefreshMenu>g__RefreshShareButton|43();
				CS$<>8__locals1.b.SetOnClick(delegate
				{
					bool flag2 = CS$<>8__locals1.data.sharedType == ContainerSharedType.Shared;
					SE.ClickOk();
					Msg.Say("changePermission", con, (flag2 ? "stPersonal" : "stShared").lang(), null, null);
					CS$<>8__locals1.data.sharedType = (flag2 ? ContainerSharedType.Personal : ContainerSharedType.Shared);
					CS$<>8__locals1.<RefreshMenu>g__RefreshShareButton|43();
				});
			}
		}
		UIInventory.Mode mode = this.currentTab.mode;
		WindowMenu menuBottom = this.window.menuBottom;
		menuBottom.Clear();
		if (mode != UIInventory.Mode.Buy)
		{
			if (mode == UIInventory.Mode.Take && !this.owner.Container.isNPCProperty && !this.owner.Container.isChara && !this.IsMagicChest)
			{
				this.buttonTakeAll = menuBottom.AddButton("takeAll".lang(EInput.keys.getAll.key.ToString().ToLower(), null, null, null, null), delegate(UIButton b)
				{
					CS$<>8__locals1.<>4__this.owner.Container.things.ForeachReverse(delegate(Thing t)
					{
						if (!EMono.pc.things.IsFull(t, true, true))
						{
							EMono.pc.Pick(t, false, true);
						}
					});
					if (CS$<>8__locals1.<>4__this.owner.Container.things.Count > 0)
					{
						SE.Beep();
						return;
					}
					SE.Play("pick_thing");
					CS$<>8__locals1.<>4__this.Close();
				}, null, "Default");
				return;
			}
		}
		else
		{
			Card _owner = this.owner.owner;
			int cost = _owner.trait.CostRerollShop;
			if (cost > 0)
			{
				menuBottom.AddButton("rerollShop".lang(cost.ToString() ?? "", null, null, null, null), delegate(UIButton b)
				{
					if (EMono._zone.influence < cost)
					{
						SE.Beep();
						Msg.Say("notEnoughInfluence");
						return;
					}
					SE.Dice();
					EMono._zone.influence -= cost;
					_owner.c_dateStockExpire = 0;
					_owner.trait.OnBarter();
					CS$<>8__locals1.<>4__this.RefreshGrid();
					CS$<>8__locals1.<>4__this.Sort(true);
					SE.Play("shop_open");
				}, null, "Default");
			}
		}
	}

	public Transform ShowDistribution(UIContextMenu dis, Window.SaveData data)
	{
		UIInventory.<>c__DisplayClass70_0 CS$<>8__locals1 = new UIInventory.<>c__DisplayClass70_0();
		CS$<>8__locals1.data = data;
		CS$<>8__locals1.items = new List<UIContextMenuItem>();
		CS$<>8__locals1.itemAll = null;
		List<ContainerFlag> list = Util.EnumToList<ContainerFlag>();
		list.Remove(ContainerFlag.none);
		GridLayoutGroup gridLayoutGroup = dis.AddGridLayout("Context_LayoutDistribution");
		CS$<>8__locals1.itemAll = dis.AddToggle("all", false, delegate(bool a)
		{
			foreach (UIContextMenuItem uicontextMenuItem2 in CS$<>8__locals1.items)
			{
				uicontextMenuItem2.toggle.isOn = a;
			}
			base.<ShowDistribution>g__Refresh|0();
			SE.ClickOk();
		});
		using (List<ContainerFlag>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ContainerFlag f = enumerator.Current;
				SourceCategory.Row row = EMono.sources.categories.map[f.ToString()];
				CS$<>8__locals1.items.Add(dis.AddToggle(row.GetName(), !CS$<>8__locals1.data.flag.HasFlag(f), delegate(bool a)
				{
					CS$<>8__locals1.<ShowDistribution>g__SetOn|1(f, !a);
					CS$<>8__locals1.<ShowDistribution>g__Refresh|0();
					SE.ClickOk();
				}));
			}
		}
		CS$<>8__locals1.itemAll.transform.SetParent(gridLayoutGroup.transform);
		CS$<>8__locals1.itemAll.GetComponentInChildren<UIText>().SetText("all".lang(), FontColor.Topic);
		foreach (UIContextMenuItem uicontextMenuItem in CS$<>8__locals1.items)
		{
			uicontextMenuItem.transform.SetParent(gridLayoutGroup.transform);
		}
		CS$<>8__locals1.<ShowDistribution>g__Refresh|0();
		return gridLayoutGroup.transform;
	}

	public Transform ShowAdvDistribution(UIContextMenu dis, Window.SaveData data)
	{
		UIDistribution uidistribution = Util.Instantiate<UIDistribution>("UI/Layer/UIDistribution", null);
		uidistribution.SetContainer(this.owner.Container, data);
		return dis.AddGameObject<UIDistribution>(uidistribution).transform;
	}

	public void Sort(bool redraw = true)
	{
		UIList.SortMode m = this.IsShop ? EMono.player.pref.sortInvShop : EMono.player.pref.sortInv;
		bool flag = true;
		while (flag)
		{
			flag = false;
			foreach (Thing thing in this.owner.Container.things)
			{
				if (thing.invY != 1)
				{
					foreach (Thing thing2 in this.owner.Container.things)
					{
						if (thing != thing2 && thing2.invY != 1 && thing.TryStackTo(thing2))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
		int num = 0;
		foreach (Thing thing3 in this.owner.Container.things)
		{
			if (thing3.invY != 1)
			{
				thing3.invY = 0;
				thing3.invX = -1;
			}
			thing3.SetSortVal(m, this.owner.currency);
			num++;
		}
		this.owner.Container.things.Sort(delegate(Thing a, Thing b)
		{
			bool flag2 = this.IsShop ? EMono.player.pref.sort_ascending_shop : EMono.player.pref.sort_ascending;
			if (m == UIList.SortMode.ByName)
			{
				if (flag2)
				{
					return string.Compare(a.GetName(NameStyle.FullNoArticle, 1), b.GetName(NameStyle.FullNoArticle, 1));
				}
				return string.Compare(b.GetName(NameStyle.FullNoArticle, 1), a.GetName(NameStyle.FullNoArticle, 1));
			}
			else
			{
				if (a.sortVal == b.sortVal)
				{
					return b.SecondaryCompare(m, a);
				}
				if (!flag2)
				{
					return a.sortVal - b.sortVal;
				}
				return b.sortVal - a.sortVal;
			}
		});
		if (!this.UseGrid)
		{
			int num2 = 0;
			int num3 = 0;
			Vector2 sizeDelta = this.list.Rect().sizeDelta;
			sizeDelta.x -= 60f;
			sizeDelta.y -= 60f;
			foreach (Thing thing4 in this.owner.Container.things)
			{
				if (thing4.invY == 0)
				{
					thing4.posInvX = num2 + 30;
					thing4.posInvY = (int)sizeDelta.y - num3 + 30;
					num2 += 40;
					if ((float)num2 > sizeDelta.x)
					{
						num2 = 0;
						num3 += 40;
						if ((float)num3 > sizeDelta.y)
						{
							num3 = 20;
						}
					}
				}
			}
		}
		if (redraw)
		{
			this.list.Redraw();
		}
	}

	public void RefreshList()
	{
		UIInventory.Mode mode = this.currentTab.mode;
		UIInventory.SubMode subMode = this.currentTab.subMode;
		this.RefreshMenu();
		this.list.Clear();
		UIList.Callback<Thing, ButtonGrid> callbacks = new UIList.Callback<Thing, ButtonGrid>
		{
			onClick = delegate(Thing a, ButtonGrid b)
			{
				if (this.destNum != 0)
				{
					if (this.destNum != -1)
					{
						int num = this.destNum;
					}
					else
					{
						int num2 = a.Num;
					}
				}
				if (this.actionClick != null)
				{
					this.actionClick(a);
					this.Close();
					return;
				}
				EMono.pc.HoldCard(a, -1);
				SE.SelectHotitem();
			},
			onInstantiate = delegate(Thing a, ButtonGrid b)
			{
				this.dirty = false;
				string name = a.Name;
				bool flag = mode == UIInventory.Mode.Buy || mode == UIInventory.Mode.Sell || mode == UIInventory.Mode.Identify;
				int num = (this.destNum == -1) ? a.Num : 1;
				int cost = flag ? (a.GetPrice(this.CurrencyType, mode == UIInventory.Mode.Sell, PriceType.Default, null) * num) : 0;
				bool canPay = cost < EMono.pc.GetCurrency(this.IDCurrency);
				if (mode == UIInventory.Mode.Identify)
				{
					cost = 100;
				}
				b.SetCard(a, ButtonGrid.Mode.Default, delegate(UINote n)
				{
					UIInventory.Mode mode = mode;
					if (mode - UIInventory.Mode.Buy <= 1)
					{
						n.Space(8, 1);
						UIItem uiitem = n.AddExtra<UIItem>("costPrice");
						uiitem.text1.SetText(Lang._currency(cost, false, 14), canPay ? FontColor.Good : FontColor.Bad);
						uiitem.image1.sprite = SpriteSheet.Get("icon_" + this.IDCurrency);
					}
				});
				if (flag && b.subText)
				{
					b.subText.SetText((cost.ToString() ?? "").TagColorGoodBad(() => mode == UIInventory.Mode.Sell || cost <= EMono.pc.GetCurrency(this.IDCurrency), false));
				}
			},
			onList = delegate(UIList.SortMode m)
			{
				List<Thing> list = new List<Thing>();
				foreach (Thing thing in this.owner.Things)
				{
					if (thing != null && (this.funcList == null || this.funcList(thing)) && !thing.HasTag(CTAG.noDrop) && (thing.c_equippedSlot == 0 || mode == UIInventory.Mode.Equip || mode == UIInventory.Mode.Identify) && (!(thing.category.id == "currency") || (mode != UIInventory.Mode.Buy && mode != UIInventory.Mode.Sell)))
					{
						list.Add(thing);
					}
				}
				int num = 0;
				UIList uilist = this.list;
				foreach (Thing thing2 in list)
				{
					num += thing2.ChildrenAndSelfWeight;
					this.list.Add(thing2);
				}
				string text = "statsInv".lang(this.owner.Container.things.Count.ToString() ?? "", this.list.ItemCount.ToString() ?? "", Lang._weight(num, true, 0), null, null);
				this.layer.windows[0].textStats.SetText(text);
			},
			onSort = null
		};
		this.list.callbacks = callbacks;
		this.list.List(false);
	}

	public void RefreshHighlight()
	{
		if (!this.imageHighlight)
		{
			return;
		}
		this.imageHighlight.SetActive(false);
		if (!EMono.game.altInv)
		{
			return;
		}
		Thing thing = EMono.player.currentHotItem.Thing;
		if (this.currentTab.mode == UIInventory.Mode.All && thing != null && !thing.isDestroyed)
		{
			ICardParent parent = thing.parent;
			Card container = this.owner.Container;
		}
	}

	public void RefreshGrid()
	{
		UIInventory.Mode mode = this.currentTab.mode;
		UIInventory.SubMode subMode = this.currentTab.subMode;
		CoreRef.InventoryStyle style = this.InvStyle;
		this.RefreshMenu();
		GridLayoutGroup g = this.list.GetComponent<GridLayoutGroup>();
		ContentSizeFitter component = this.list.GetComponent<ContentSizeFitter>();
		if (this.UseGrid)
		{
			g.constraintCount = ((this.window.saveData.columns == 0) ? this.owner.Container.things.width : this.window.saveData.columns);
			Vector2 cellSize = style.gridSize * (float)(100 + this.window.saveData.size) / 100f;
			g.cellSize = cellSize;
			g.enabled = (component.enabled = true);
		}
		else
		{
			g.enabled = (component.enabled = false);
			this.list.Rect().sizeDelta = style.sizeContainer;
		}
		this.list.Clear();
		int maxCapacity = this.IsMagicChest ? this.owner.Container.things.MaxCapacity : 0;
		UIList.Callback<Thing, ButtonGrid> callback = new UIList.Callback<Thing, ButtonGrid>
		{
			onRedraw = delegate(Thing a, ButtonGrid b, int i)
			{
				this.dirty = false;
				b.index = i;
				bool flag = i >= this.owner.Things.Count;
				if (this.IsMagicChest)
				{
					flag = (this.uiMagic.page * this.uiMagic.GridSize + i >= maxCapacity);
				}
				if (this.UseGrid)
				{
					b.SetCardGrid(a, flag ? null : this.owner);
				}
				else
				{
					b.SetCardGrid(a, this.owner);
					if (a != null)
					{
						b.Rect().anchoredPosition = new Vector2((float)a.posInvX, (float)a.posInvY);
					}
					float num = 0.01f * (float)EMono.game.config.gridIconSize;
					b.icon.Rect().localScale = new Vector3(num, num, 1f);
					b.Rect().sizeDelta = b.icon.rectTransform.sizeDelta;
				}
				if (flag)
				{
					b.interactable = false;
					b.image.sprite = null;
					b.image.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 120);
					b.image.raycastTarget = true;
					return;
				}
				b.image.raycastTarget = true;
			},
			onList = delegate(UIList.SortMode m)
			{
				this.list.bgType = (this.UseGrid ? UIList.BGType.grid : UIList.BGType.none);
				if (this.list.bgGrid)
				{
					this.list.bgGrid.SetActive(this.UseGrid);
					this.list.bgGrid.color = style.gridColor;
					if (this.window.saveData.color.a != 0)
					{
						this.list.bgGrid.color = this.window.saveData.color;
					}
				}
				if (this.IsMagicChest)
				{
					this.owner.Container.things.RefreshGrid(this.uiMagic, this.window.saveData);
				}
				else
				{
					this.owner.Container.things.RefreshGrid();
				}
				if (this.UseGrid)
				{
					if (this.IsMagicChest)
					{
						for (int i = 0; i < this.owner.Container.things.GridSize; i++)
						{
							this.list.Add(this.owner.Things[i]);
						}
					}
					else
					{
						int count = this.owner.Things.Count;
						int num = (int)Mathf.Ceil((float)(count / g.constraintCount + ((count % g.constraintCount == 0) ? 0 : 1))) * g.constraintCount;
						for (int j = 0; j < num; j++)
						{
							this.list.Add((j < count) ? this.owner.Things[j] : null);
						}
					}
				}
				else
				{
					Vector2 sizeDelta2 = this.list.Rect().sizeDelta;
					foreach (Thing thing2 in this.owner.Things)
					{
						if (thing2 != null)
						{
							this.list.Add(thing2);
							if (thing2.posInvX == 0 && thing2.posInvY == 0)
							{
								thing2.posInvX = EMono.rnd((int)sizeDelta2.x - 60) + 30;
								thing2.posInvY = EMono.rnd((int)sizeDelta2.y - 60) + 30;
							}
						}
					}
				}
				if (this.floatMode)
				{
					this.window.setting.tabs[0].idLang = "captionInvFloat".lang(this.owner.Container.IsPC ? "" : this.owner.Container.Name, ((float)this.owner.Container.ChildrenWeight / 1000f).ToString("F1") + "/" + ((float)this.owner.Container.WeightLimit / 1000f).ToString("F1"), null, null, null);
				}
			}
		};
		bool sorted = false;
		this.list.callbacks = callback;
		this.list.onBeforeRedraw = delegate()
		{
			if (this.IsMagicChest)
			{
				if (this.firstMouseRightDown || (!Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)))
				{
					this.Sort(false);
					this.firstMouseRightDown = false;
					this.owner.Container.things.RefreshGrid(this.uiMagic, this.window.saveData);
				}
			}
			else
			{
				if (this.window.saveData.alwaysSort && !sorted && (this.firstMouseRightDown || (!Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))))
				{
					sorted = true;
					this.Sort(true);
					sorted = false;
					this.firstMouseRightDown = false;
				}
				this.owner.Container.things.RefreshGrid();
			}
			this.dirty = false;
			if (this.UseGrid)
			{
				UIList uilist = this.list;
				for (int i = 0; i < uilist.buttons.Count; i++)
				{
					UIList.ButtonPair value = uilist.buttons[i];
					value.obj = ((i < this.owner.Container.things.grid.Count) ? this.owner.Container.things.grid[i] : null);
					uilist.buttons[i] = value;
				}
			}
		};
		this.list.onAfterRedraw = delegate()
		{
			if (!this.layer.floatInv)
			{
				int num = 0;
				foreach (UIList.ButtonPair buttonPair in this.list.buttons)
				{
					Thing thing2 = (buttonPair.component as ButtonGrid).card as Thing;
					if (thing2 != null)
					{
						num += thing2.ChildrenAndSelfWeight;
					}
				}
				string text = "statsInv2".lang(this.owner.Container.things.Count.ToString() ?? "", this.list.ItemCount.ToString() ?? "", Lang._weight(num, true, 0) + "/" + Lang._weight(this.owner.Container.WeightLimit, true, 0), null, null);
				this.layer.windows[0].textStats.SetText(text);
			}
			if (this.owner.Container.things.IsOverflowing())
			{
				if (!this.transOverflow)
				{
					this.transOverflow = Util.Instantiate("UI/Element/Other/InvOverflow", this.window.transform);
				}
			}
			else if (this.transOverflow)
			{
				UnityEngine.Object.Destroy(this.transOverflow.gameObject);
			}
			LayerInventory.TryShowGuide(this.list);
			if (this.layer.mini)
			{
				this.layer.mini.Refresh(this.layer.mini.window.idTab);
			}
			if (this.uiMagic)
			{
				this.uiMagic.OnAfterRedraw();
			}
			UIButton.TryShowTip(null, true, true);
		};
		callback.mold = (this.UseGrid ? this.moldButtonGrid : this.moldButtonGridless);
		if (!this.UseGrid && !this.window.saveData.firstSorted)
		{
			Vector2 sizeDelta = this.list.Rect().sizeDelta;
			foreach (Thing thing in this.owner.Things)
			{
				if (thing != null)
				{
					thing.posInvX = EMono.rnd((int)sizeDelta.x - 60) + 30;
					thing.posInvY = EMono.rnd((int)sizeDelta.y - 60) + 30;
				}
			}
			this.window.saveData.firstSorted = true;
		}
		this.list.List(false);
		g.RebuildLayoutTo<Layer>();
		this.list.onAfterRedraw();
		if (this.list.bgGrid)
		{
			this.list.bgGrid.color = style.gridColor;
			if (this.window.saveData.color.a != 0)
			{
				this.list.bgGrid.color = this.window.saveData.color;
			}
		}
		if (this.owner == InvOwner.Trader && this.owner.UseGuide)
		{
			LayerInventory.SetDirtyAll(false);
		}
	}

	public UIList list;

	public BodySlot bodySlot;

	public LayerInventory layer;

	public Transform headerRow;

	public Transform transOverflow;

	public string idMold;

	public Func<Thing, bool> funcList;

	public Action<Thing> actionClick;

	public Window window;

	public UIContent content;

	public UIInventory.Tab currentTab;

	public AIAct currentAct;

	public UIInventory.InteractMode interactMode;

	public bool floatMode;

	public bool dirty;

	public bool isList;

	public UIInventory.Transaction lastTransaction;

	public Image imageHighlight;

	public Image imageHighlightGrid;

	public UIButton moldSmallTab;

	public ButtonGrid moldButtonGrid;

	public ButtonGrid moldButtonGridless;

	public UIButton buttonTakeAll;

	public UIMagicChest uiMagic;

	private int destNum;

	private int lastNum;

	private bool firstMouseRightDown;

	public List<UIInventory.Tab> tabs = new List<UIInventory.Tab>();

	public bool wasDirty;

	public class Transaction
	{
		public Thing thing;

		public int num;

		public int price;

		public UIInventory.Mode mode;

		public Card from;
	}

	public enum Mode
	{
		Default,
		Equip,
		Buy,
		Sell,
		Gear,
		Other,
		Select,
		Food,
		Item,
		Resource,
		Read,
		Hold,
		Tool,
		Put,
		Take,
		Give,
		Trade,
		All,
		Offer,
		Identify,
		Drink,
		Deliver,
		HoldFurniture,
		HoldBlock,
		Recycle
	}

	public enum SubMode
	{
		Default,
		IdentifyScroll,
		DeliverTax
	}

	public enum InteractMode
	{
		Auto,
		Hold,
		Drop
	}

	public class Tab
	{
		public Card dest;

		public InvOwner owner;

		public UIInventory.Mode mode;

		public UIInventory.SubMode subMode;

		public string textTab;

		public bool smallTab;
	}
}
